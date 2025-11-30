/* ==========================================================================
   Multi-Step License Application Form
   A production-ready application with state management, validation, and auto-save
   ========================================================================== */

// ==========================================================================
// Configuration
// ==========================================================================
const CONFIG = {
    API_BASE_URL: window.location.origin + '/api',
    AUTO_SAVE_INTERVAL: 60000, // 60 seconds
    MAX_FILE_SIZE: 10 * 1024 * 1024, // 10MB
    ALLOWED_FILE_TYPES: ['.pdf', '.jpg', '.jpeg', '.png', '.docx', '.doc']
};

// ==========================================================================
// State Management
// ==========================================================================
class ApplicationState {
    constructor() {
        this.currentStep = 1;
        this.totalSteps = 5;
        this.formData = this.loadFromStorage() || {
            accountType: 1,
            accountName: '',
            email: '',
            phone: '',
            firstName: '',
            lastName: '',
            dateOfBirth: null,
            socialSecurityNumber: '',
            businessName: '',
            taxId: '',
            businessEstablishedDate: null,
            businessType: '',
            organizationName: '',
            organizationType: '',
            registrationNumber: '',
            streetAddress: '',
            addressLine2: '',
            city: '',
            state: '',
            zipCode: '',
            country: 'India',
            licenseType: '',
            hasPreviousLicense: false,
            previousLicenseNumber: '',
            previousLicenseExpiry: null,
            licensePurpose: '',
            agreeToTerms: false,
            additionalNotes: '',
            uploadedFiles: []
        };
        this.accountTypes = [];
        this.validationErrors = {};
        this.isDirty = false;
        this.isSaving = false;
        this.lastSaved = null;
        this.applicationId = null;
    }

    updateField(field, value) {
        this.formData[field] = value;
        this.isDirty = true;
        this.saveToStorage();
    }

    updateMultipleFields(fields) {
        Object.assign(this.formData, fields);
        this.isDirty = true;
        this.saveToStorage();
    }

    getField(field) {
        return this.formData[field];
    }

    getAllData() {
        return { ...this.formData };
    }

    saveToStorage() {
        try {
            localStorage.setItem('applicationFormData', JSON.stringify(this.formData));
        } catch (e) {
            console.error('Error saving to localStorage:', e);
        }
    }

    loadFromStorage() {
        try {
            const data = localStorage.getItem('applicationFormData');
            return data ? JSON.parse(data) : null;
        } catch (e) {
            console.error('Error loading from localStorage:', e);
            return null;
        }
    }

    clearStorage() {
        try {
            localStorage.removeItem('applicationFormData');
            this.isDirty = false;
        } catch (e) {
            console.error('Error clearing localStorage:', e);
        }
    }

    setValidationErrors(errors) {
        this.validationErrors = errors;
    }

    clearValidationErrors() {
        this.validationErrors = {};
    }

    hasValidationErrors() {
        return Object.keys(this.validationErrors).length > 0;
    }
}

// ==========================================================================
// API Client
// ==========================================================================
class ApiClient {
    async get(endpoint) {
        try {
            const response = await fetch(`${CONFIG.API_BASE_URL}${endpoint}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            return await this.handleResponse(response);
        } catch (error) {
            console.error('API GET Error:', error);
            throw error;
        }
    }

    async post(endpoint, data) {
        try {
            const response = await fetch(`${CONFIG.API_BASE_URL}${endpoint}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            return await this.handleResponse(response);
        } catch (error) {
            console.error('API POST Error:', error);
            throw error;
        }
    }

    async put(endpoint, data) {
        try {
            const response = await fetch(`${CONFIG.API_BASE_URL}${endpoint}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            return await this.handleResponse(response);
        } catch (error) {
            console.error('API PUT Error:', error);
            throw error;
        }
    }

    async uploadFile(file, applicationId) {
        try {
            const formData = new FormData();
            formData.append('file', file);
            if (applicationId) {
                formData.append('applicationId', applicationId);
            }

            const response = await fetch(`${CONFIG.API_BASE_URL}/files/upload`, {
                method: 'POST',
                body: formData
            });
            return await this.handleResponse(response);
        } catch (error) {
            console.error('File Upload Error:', error);
            throw error;
        }
    }

    async handleResponse(response) {
        const data = await response.json();

        if (!response.ok) {
            // Log the full error data for debugging
            console.error('API Error Response:', data);

            // If validation errors exist, log them
            if (data.errors) {
                console.error('Validation Errors:', data.errors);
            }

            throw new Error(data.message || 'API request failed');
        }

        return data;
    }
}

// ==========================================================================
// Form Controller
// ==========================================================================
class FormController {
    constructor() {
        this.state = new ApplicationState();
        this.api = new ApiClient();
        this.autoSaveTimer = null;
        this.init();
    }

    async init() {
        // Load account types from API
        await this.loadAccountTypes();

        // Set up event listeners
        this.setupEventListeners();

        // Initialize form with saved data
        this.populateFormFromState();

        // Start auto-save
        this.startAutoSave();

        // Update UI
        this.updateProgressIndicator();
        this.updateNavigationButtons();
        this.updateAutoSaveIndicator();
    }

    async loadAccountTypes() {
        try {
            const accountTypes = await this.api.get('/accounttypes');
            this.state.accountTypes = accountTypes;
            this.renderAccountTypeCards();
        } catch (error) {
            console.error('Error loading account types:', error);
            this.showError('Failed to load account types');
        }
    }

    setupEventListeners() {
        // Navigation buttons
        document.getElementById('nextBtn').addEventListener('click', () => this.nextStep());
        document.getElementById('prevBtn').addEventListener('click', () => this.prevStep());
        document.getElementById('submitBtn').addEventListener('click', () => this.submitApplication());
        document.getElementById('saveDraftBtn').addEventListener('click', () => this.saveDraft());

        // Form fields change detection
        document.querySelectorAll('.form-control, input[type="radio"], input[type="checkbox"]').forEach(field => {
            field.addEventListener('change', (e) => this.handleFieldChange(e));
            field.addEventListener('blur', (e) => this.validateField(e.target));
        });

        // Previous license conditional fields
        document.querySelectorAll('input[name="hasPreviousLicense"]').forEach(radio => {
            radio.addEventListener('change', () => this.togglePreviousLicenseFields());
        });

        // File upload
        this.setupFileUpload();

        // Modal buttons
        document.getElementById('printSummaryBtn').addEventListener('click', () => this.printSummary());
        document.getElementById('newApplicationBtn').addEventListener('click', () => this.resetForm());

        // Progress steps click navigation
        document.querySelectorAll('.progress-step').forEach((step, index) => {
            step.addEventListener('click', () => {
                if (index < this.state.currentStep) {
                    this.goToStep(index + 1);
                }
            });
        });
    }

    setupFileUpload() {
        const uploadZone = document.getElementById('fileUploadZone');
        const fileInput = document.getElementById('fileInput');

        // Click to upload
        uploadZone.addEventListener('click', () => fileInput.click());

        // File input change
        fileInput.addEventListener('change', (e) => {
            this.handleFiles(Array.from(e.target.files));
            e.target.value = ''; // Reset input
        });

        // Drag and drop
        uploadZone.addEventListener('dragover', (e) => {
            e.preventDefault();
            uploadZone.classList.add('drag-over');
        });

        uploadZone.addEventListener('dragleave', () => {
            uploadZone.classList.remove('drag-over');
        });

        uploadZone.addEventListener('drop', (e) => {
            e.preventDefault();
            uploadZone.classList.remove('drag-over');
            this.handleFiles(Array.from(e.dataTransfer.files));
        });
    }

    async handleFiles(files) {
        const fileList = document.getElementById('fileList');

        // Ensure we have an application ID by saving a draft first if needed
        if (!this.state.applicationId || this.state.applicationId === '00000000-0000-0000-0000-000000000000') {
            try {
                // Save draft silently (no success message)
                await this.saveDraft(true);
            } catch (error) {
                console.error('Failed to save draft before uploading files:', error);
                this.showError('Please fill in required fields before uploading files');
                return;
            }
        }

        for (const file of files) {
            // Validate file
            if (!this.validateFile(file)) {
                continue;
            }

            // Show file in list with uploading state
            const fileItem = this.createFileItem(file, 'uploading');
            fileList.appendChild(fileItem);

            try {
                // Upload file - application ID is now guaranteed to exist
                const response = await this.api.uploadFile(file, this.state.applicationId);

                // Update file item to show success
                fileItem.classList.remove('uploading');
                fileItem.classList.add('uploaded');
                fileItem.dataset.fileId = response.id;

                // Add to state
                this.state.formData.uploadedFiles.push({
                    id: response.id,
                    fileName: response.fileName,
                    originalFileName: response.originalFileName,
                    fileSize: response.fileSize
                });

                this.state.saveToStorage();
            } catch (error) {
                console.error('File upload error:', error);
                fileItem.classList.add('error');
                this.showError(`Failed to upload ${file.name}`);
            }
        }
    }

    validateFile(file) {
        const extension = '.' + file.name.split('.').pop().toLowerCase();

        if (!CONFIG.ALLOWED_FILE_TYPES.includes(extension)) {
            this.showError(`File type ${extension} is not allowed`);
            return false;
        }

        if (file.size > CONFIG.MAX_FILE_SIZE) {
            this.showError(`File ${file.name} exceeds maximum size of 10MB`);
            return false;
        }

        return true;
    }

    createFileItem(file, status = 'uploaded') {
        const div = document.createElement('div');
        div.className = `file-item ${status}`;

        const fileName = file.name || 'Unknown';
        const extension = fileName.split('.').pop().toUpperCase();

        div.innerHTML = `
            <div class="file-info">
                <div class="file-icon">${extension}</div>
                <div class="file-details">
                    <div class="file-name">${fileName}</div>
                    <div class="file-size">${this.formatFileSize(file.size || 0)}</div>
                </div>
            </div>
            <button type="button" class="file-remove" onclick="formController.removeFile(this)">×</button>
        `;

        return div;
    }

    removeFile(button) {
        const fileItem = button.closest('.file-item');
        const fileId = fileItem.dataset.fileId;

        if (fileId) {
            // Remove from state
            this.state.formData.uploadedFiles = this.state.formData.uploadedFiles.filter(
                f => f.id !== fileId
            );
            this.state.saveToStorage();
        }

        fileItem.remove();
    }

    formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
    }

    renderAccountTypeCards() {
        const container = document.getElementById('accountTypeCards');
        container.innerHTML = '';

        this.state.accountTypes.forEach(type => {
            const card = document.createElement('div');
            card.className = 'account-type-card';
            card.dataset.accountType = type.type;

            if (type.type === this.state.formData.accountType) {
                card.classList.add('selected');
            }

            card.innerHTML = `
                <h3>${type.name}</h3>
                <p>${type.description}</p>
            `;

            card.addEventListener('click', () => {
                document.querySelectorAll('.account-type-card').forEach(c => c.classList.remove('selected'));
                card.classList.add('selected');
                this.state.updateField('accountType', type.type);
                this.generateDynamicFields(type);
            });

            container.appendChild(card);
        });

        // Generate fields for currently selected type
        const selectedType = this.state.accountTypes.find(t => t.type === this.state.formData.accountType);
        if (selectedType) {
            this.generateDynamicFields(selectedType);
        }
    }

    generateDynamicFields(accountType) {
        const container = document.getElementById('dynamicFields');
        container.innerHTML = '';

        const step2Fields = accountType.fields.filter(f => f.step === 2);

        step2Fields.forEach(field => {
            const formGroup = document.createElement('div');
            formGroup.className = 'form-group';

            let inputHtml = '';

            switch (field.type) {
                case 'text':
                case 'date':
                    inputHtml = `
                        <label for="${field.name}" class="${field.required ? 'required' : ''}">${field.label}</label>
                        <input type="${field.type}"
                               id="${field.name}"
                               name="${field.name}"
                               class="form-control"
                               placeholder="${field.placeholder || ''}"
                               ${field.required ? 'required' : ''}
                               value="${this.state.formData[field.name] || ''}">
                        <span class="error-message"></span>
                    `;
                    break;

                case 'select':
                    inputHtml = `
                        <label for="${field.name}" class="${field.required ? 'required' : ''}">${field.label}</label>
                        <select id="${field.name}"
                                name="${field.name}"
                                class="form-control"
                                ${field.required ? 'required' : ''}>
                            <option value="">Select ${field.label}</option>
                            ${field.options.map(opt =>
                                `<option value="${opt}" ${this.state.formData[field.name] === opt ? 'selected' : ''}>${opt}</option>`
                            ).join('')}
                        </select>
                        <span class="error-message"></span>
                    `;
                    break;
            }

            formGroup.innerHTML = inputHtml;
            container.appendChild(formGroup);

            // Add event listeners to new fields
            const input = formGroup.querySelector('.form-control');
            if (input) {
                input.addEventListener('change', (e) => this.handleFieldChange(e));
                input.addEventListener('blur', (e) => this.validateField(e.target));
            }
        });
    }

    handleFieldChange(event) {
        const field = event.target;
        const name = field.name;
        let value;

        if (field.type === 'checkbox') {
            value = field.checked;
        } else if (field.type === 'radio') {
            value = field.value === 'true';
        } else {
            value = field.value;
        }

        this.state.updateField(name, value);
        this.clearFieldError(field);
    }

    populateFormFromState() {
        const data = this.state.formData;

        // Populate all form fields
        Object.keys(data).forEach(key => {
            const field = document.querySelector(`[name="${key}"]`);

            if (field) {
                if (field.type === 'checkbox') {
                    field.checked = data[key];
                } else if (field.type === 'radio') {
                    const radio = document.querySelector(`[name="${key}"][value="${data[key]}"]`);
                    if (radio) radio.checked = true;
                } else {
                    field.value = data[key] || '';
                }
            }
        });

        // Update conditional fields
        this.togglePreviousLicenseFields();

        // Render uploaded files
        this.renderUploadedFiles();
    }

    renderUploadedFiles() {
        const fileList = document.getElementById('fileList');
        fileList.innerHTML = '';

        if (this.state.formData.uploadedFiles && this.state.formData.uploadedFiles.length > 0) {
            this.state.formData.uploadedFiles.forEach(file => {
                const fileItem = this.createFileItem({
                    name: file.fileName || file.originalFileName,
                    size: file.fileSize
                }, 'uploaded');
                fileItem.dataset.fileId = file.id;
                fileList.appendChild(fileItem);
            });
        }
    }

    togglePreviousLicenseFields() {
        const hasPrevious = document.querySelector('input[name="hasPreviousLicense"]:checked')?.value === 'true';
        const fieldsContainer = document.getElementById('previousLicenseFields');

        if (hasPrevious) {
            fieldsContainer.style.display = 'block';
            fieldsContainer.querySelectorAll('input').forEach(input => input.required = true);
        } else {
            fieldsContainer.style.display = 'none';
            fieldsContainer.querySelectorAll('input').forEach(input => {
                input.required = false;
                input.value = '';
            });
        }
    }

    async nextStep() {
        // Validate current step
        if (!await this.validateCurrentStep()) {
            this.showValidationSummary();
            return;
        }

        if (this.state.currentStep < this.state.totalSteps) {
            this.state.currentStep++;
            this.goToStep(this.state.currentStep);
        }
    }

    prevStep() {
        if (this.state.currentStep > 1) {
            this.state.currentStep--;
            this.goToStep(this.state.currentStep);
        }
    }

    goToStep(stepNumber) {
        this.state.currentStep = stepNumber;

        // Hide all steps
        document.querySelectorAll('.form-step').forEach(step => {
            step.classList.remove('active');
        });

        // Show current step
        const currentStepElement = document.querySelector(`.form-step[data-step="${stepNumber}"]`);
        if (currentStepElement) {
            currentStepElement.classList.add('active');
        }

        // Update progress indicator
        this.updateProgressIndicator();

        // Update navigation buttons
        this.updateNavigationButtons();

        // If review step, generate review content
        if (stepNumber === 5) {
            this.generateReviewContent();
        }

        // Scroll to top
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    updateProgressIndicator() {
        // Update step indicators
        document.querySelectorAll('.progress-step').forEach((step, index) => {
            const stepNumber = index + 1;

            if (stepNumber < this.state.currentStep) {
                step.classList.add('completed');
                step.classList.remove('active');
            } else if (stepNumber === this.state.currentStep) {
                step.classList.add('active');
                step.classList.remove('completed');
            } else {
                step.classList.remove('active', 'completed');
            }
        });

        // Update progress bar
        const progress = ((this.state.currentStep - 1) / (this.state.totalSteps - 1)) * 100;
        document.getElementById('progressBarFill').style.width = `${progress}%`;
    }

    updateNavigationButtons() {
        const prevBtn = document.getElementById('prevBtn');
        const nextBtn = document.getElementById('nextBtn');
        const submitBtn = document.getElementById('submitBtn');

        prevBtn.style.display = this.state.currentStep === 1 ? 'none' : 'inline-flex';
        nextBtn.style.display = this.state.currentStep === this.state.totalSteps ? 'none' : 'inline-flex';
        submitBtn.style.display = this.state.currentStep === this.state.totalSteps ? 'inline-flex' : 'none';
    }

    async validateCurrentStep() {
        const currentStepElement = document.querySelector(`.form-step[data-step="${this.state.currentStep}"]`);
        const fields = currentStepElement.querySelectorAll('.form-control[required], input[required], select[required]');

        let isValid = true;
        const errors = [];

        for (const field of fields) {
            if (!this.validateField(field)) {
                isValid = false;
                errors.push(field.name);
            }
        }

        // Clear previous validation errors
        this.hideValidationSummary();

        return isValid;
    }

    validateField(field) {
        if (!field) return true;

        let isValid = true;
        let errorMessage = '';

        // Required field validation
        if (field.hasAttribute('required')) {
            if (!field.value || field.value.trim() === '') {
                isValid = false;
                errorMessage = 'This field is required';
            }
        }

        // Email validation
        if (field.type === 'email' && field.value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(field.value)) {
                isValid = false;
                errorMessage = 'Please enter a valid email address';
            }
        }

        // Phone validation
        if (field.name === 'phone' && field.value) {
            const phoneRegex = /^[\d\s\(\)\-\+]+$/;
            if (!phoneRegex.test(field.value) || field.value.replace(/\D/g, '').length < 10) {
                isValid = false;
                errorMessage = 'Please enter a valid phone number';
            }
        }

        // PIN code validation (Indian PIN codes are 6 digits)
        if (field.name === 'zipCode' && field.value) {
            const pinRegex = /^\d{6}$/;
            if (!pinRegex.test(field.value)) {
                isValid = false;
                errorMessage = 'Please enter a valid PIN code (6 digits, e.g., 110001)';
            }
        }

        // Date of birth validation (must be 18+)
        if (field.name === 'dateOfBirth' && field.value) {
            const birthDate = new Date(field.value);
            const age = (new Date() - birthDate) / (365.25 * 24 * 60 * 60 * 1000);
            if (age < 18) {
                isValid = false;
                errorMessage = 'You must be at least 18 years old';
            }
        }

        // Update UI
        if (isValid) {
            field.classList.remove('error');
            field.classList.add('success');
            this.clearFieldError(field);
        } else {
            field.classList.add('error');
            field.classList.remove('success');
            this.showFieldError(field, errorMessage);
        }

        return isValid;
    }

    showFieldError(field, message) {
        const formGroup = field.closest('.form-group');
        if (formGroup) {
            const errorElement = formGroup.querySelector('.error-message');
            if (errorElement) {
                errorElement.textContent = message;
                errorElement.classList.add('show');
            }
        }
    }

    clearFieldError(field) {
        const formGroup = field.closest('.form-group');
        if (formGroup) {
            const errorElement = formGroup.querySelector('.error-message');
            if (errorElement) {
                errorElement.textContent = '';
                errorElement.classList.remove('show');
            }
        }
        field.classList.remove('error');
    }

    showValidationSummary() {
        const summary = document.getElementById('validationSummary');
        const list = document.getElementById('validationList');

        const errors = document.querySelectorAll('.form-control.error');

        if (errors.length > 0) {
            list.innerHTML = '';
            errors.forEach(field => {
                const label = field.closest('.form-group')?.querySelector('label')?.textContent || field.name;
                const li = document.createElement('li');
                li.textContent = label;
                list.appendChild(li);
            });

            summary.style.display = 'block';

            // Scroll to summary
            summary.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        }
    }

    hideValidationSummary() {
        document.getElementById('validationSummary').style.display = 'none';
    }

    generateReviewContent() {
        const container = document.getElementById('reviewContent');
        const data = this.state.formData;
        const accountType = this.state.accountTypes.find(t => t.type === data.accountType);

        let html = '';

        // Account Information
        html += `
            <div class="review-section">
                <h3>Account Information</h3>
                <div class="review-item">
                    <span class="review-label">Account Type:</span>
                    <span class="review-value">${accountType?.name || 'N/A'}</span>
                </div>
                <div class="review-item">
                    <span class="review-label">Account Name:</span>
                    <span class="review-value">${data.accountName || 'N/A'}</span>
                </div>
                <div class="review-item">
                    <span class="review-label">Email:</span>
                    <span class="review-value">${data.email || 'N/A'}</span>
                </div>
                <div class="review-item">
                    <span class="review-label">Phone:</span>
                    <span class="review-value">${data.phone || 'N/A'}</span>
                </div>
            </div>
        `;

        // Account Type Specific Details
        if (data.accountType === 1) {
            html += `
                <div class="review-section">
                    <h3>Personal Details</h3>
                    <div class="review-item">
                        <span class="review-label">Name:</span>
                        <span class="review-value">${data.firstName} ${data.lastName}</span>
                    </div>
                    <div class="review-item">
                        <span class="review-label">Date of Birth:</span>
                        <span class="review-value">${data.dateOfBirth || 'N/A'}</span>
                    </div>
                    <div class="review-item">
                        <span class="review-label">SSN:</span>
                        <span class="review-value">${data.socialSecurityNumber ? '***-**-' + data.socialSecurityNumber.slice(-4) : 'N/A'}</span>
                    </div>
                </div>
            `;
        } else if (data.accountType === 2) {
            html += `
                <div class="review-section">
                    <h3>Business Details</h3>
                    <div class="review-item">
                        <span class="review-label">Business Name:</span>
                        <span class="review-value">${data.businessName || 'N/A'}</span>
                    </div>
                    <div class="review-item">
                        <span class="review-label">Tax ID:</span>
                        <span class="review-value">${data.taxId || 'N/A'}</span>
                    </div>
                    <div class="review-item">
                        <span class="review-label">Business Type:</span>
                        <span class="review-value">${data.businessType || 'N/A'}</span>
                    </div>
                    <div class="review-item">
                        <span class="review-label">Established Date:</span>
                        <span class="review-value">${data.businessEstablishedDate || 'N/A'}</span>
                    </div>
                </div>
            `;
        } else if (data.accountType === 3) {
            html += `
                <div class="review-section">
                    <h3>Organization Details</h3>
                    <div class="review-item">
                        <span class="review-label">Organization Name:</span>
                        <span class="review-value">${data.organizationName || 'N/A'}</span>
                    </div>
                    <div class="review-item">
                        <span class="review-label">Organization Type:</span>
                        <span class="review-value">${data.organizationType || 'N/A'}</span>
                    </div>
                    <div class="review-item">
                        <span class="review-label">Registration Number:</span>
                        <span class="review-value">${data.registrationNumber || 'N/A'}</span>
                    </div>
                </div>
            `;
        }

        // Address Information
        html += `
            <div class="review-section">
                <h3>Address Information</h3>
                <div class="review-item">
                    <span class="review-label">Street Address:</span>
                    <span class="review-value">${data.streetAddress || 'N/A'}</span>
                </div>
                ${data.addressLine2 ? `
                    <div class="review-item">
                        <span class="review-label">Address Line 2:</span>
                        <span class="review-value">${data.addressLine2}</span>
                    </div>
                ` : ''}
                <div class="review-item">
                    <span class="review-label">City, State ZIP:</span>
                    <span class="review-value">${data.city}, ${data.state} ${data.zipCode}</span>
                </div>
                <div class="review-item">
                    <span class="review-label">Country:</span>
                    <span class="review-value">${data.country}</span>
                </div>
            </div>
        `;

        // License Information
        html += `
            <div class="review-section">
                <h3>License Information</h3>
                <div class="review-item">
                    <span class="review-label">License Type:</span>
                    <span class="review-value">${data.licenseType || 'N/A'}</span>
                </div>
                <div class="review-item">
                    <span class="review-label">Previous License:</span>
                    <span class="review-value">${data.hasPreviousLicense ? 'Yes' : 'No'}</span>
                </div>
                ${data.hasPreviousLicense ? `
                    <div class="review-item">
                        <span class="review-label">Previous License Number:</span>
                        <span class="review-value">${data.previousLicenseNumber || 'N/A'}</span>
                    </div>
                    <div class="review-item">
                        <span class="review-label">Expiry Date:</span>
                        <span class="review-value">${data.previousLicenseExpiry || 'N/A'}</span>
                    </div>
                ` : ''}
                ${data.licensePurpose ? `
                    <div class="review-item">
                        <span class="review-label">Purpose:</span>
                        <span class="review-value">${data.licensePurpose}</span>
                    </div>
                ` : ''}
            </div>
        `;

        // Uploaded Files
        if (data.uploadedFiles.length > 0) {
            html += `
                <div class="review-section">
                    <h3>Uploaded Documents</h3>
                    ${data.uploadedFiles.map(file => `
                        <div class="review-item">
                            <span class="review-label">${file.originalFileName}</span>
                            <span class="review-value">${this.formatFileSize(file.fileSize)}</span>
                        </div>
                    `).join('')}
                </div>
            `;
        }

        container.innerHTML = html;
    }

    async submitApplication() {
        this.showLoading();

        try {
            // Final validation
            if (!await this.validateCurrentStep()) {
                this.hideLoading();
                this.showValidationSummary();
                return;
            }

            // Check terms agreement
            if (!this.state.formData.agreeToTerms) {
                this.hideLoading();
                this.showError('You must agree to the terms and conditions');
                return;
            }

            // Prepare data for submission
            const submissionData = this.prepareSubmissionData();

            // Submit to API
            const response = await this.api.post('/applications', submissionData);

            // Clear storage
            this.state.clearStorage();

            // Show success modal
            this.showSuccessModal(response.referenceNumber);

        } catch (error) {
            console.error('Submission error:', error);
            this.showError('Failed to submit application. Please try again.');
        } finally {
            this.hideLoading();
        }
    }

    prepareSubmissionData() {
        const data = { ...this.state.formData };

        // Remove fields not needed for submission
        delete data.uploadedFiles;

        // Convert empty strings to null for optional fields
        Object.keys(data).forEach(key => {
            if (data[key] === '' || data[key] === undefined) {
                data[key] = null;
            }
        });

        // Convert string booleans to actual booleans
        if (typeof data.hasPreviousLicense === 'string') {
            data.hasPreviousLicense = data.hasPreviousLicense === 'true';
        }

        // Format dates if needed - only convert valid dates
        if (data.dateOfBirth) {
            const date = new Date(data.dateOfBirth);
            if (!isNaN(date.getTime())) {
                data.dateOfBirth = date.toISOString();
            } else {
                data.dateOfBirth = null;
            }
        }
        if (data.businessEstablishedDate) {
            const date = new Date(data.businessEstablishedDate);
            if (!isNaN(date.getTime())) {
                data.businessEstablishedDate = date.toISOString();
            } else {
                data.businessEstablishedDate = null;
            }
        }
        if (data.previousLicenseExpiry) {
            const date = new Date(data.previousLicenseExpiry);
            if (!isNaN(date.getTime())) {
                data.previousLicenseExpiry = date.toISOString();
            } else {
                data.previousLicenseExpiry = null;
            }
        }

        return data;
    }

    async saveDraft(silent = false) {
        if (this.state.isSaving) return;

        this.state.isSaving = true;
        this.updateAutoSaveIndicator('saving');

        try {
            const draftData = this.prepareSubmissionData();

            let response;

            // If no applicationId exists, create a new draft
            if (!this.state.applicationId) {
                response = await this.api.post('/applications/draft', draftData);
            } else {
                // Update existing draft
                response = await this.api.put(`/applications/${this.state.applicationId}/draft`, draftData);
            }

            this.state.applicationId = response.id;
            this.state.lastSaved = new Date();
            this.state.isDirty = false;

            this.updateAutoSaveIndicator('saved');

            // Only show success message if not silent
            if (!silent) {
                this.showSuccess('Draft saved successfully');
            }

        } catch (error) {
            console.error('Draft save error:', error);
            if (!silent) {
                this.showError('Failed to save draft');
            }
            this.updateAutoSaveIndicator();
            throw error; // Re-throw so caller knows it failed
        } finally {
            this.state.isSaving = false;
        }
    }

    startAutoSave() {
        this.autoSaveTimer = setInterval(() => {
            if (this.state.isDirty && !this.state.isSaving) {
                // Auto-save silently (no success message)
                this.saveDraft(true);
            }
        }, CONFIG.AUTO_SAVE_INTERVAL);
    }

    stopAutoSave() {
        if (this.autoSaveTimer) {
            clearInterval(this.autoSaveTimer);
        }
    }

    updateAutoSaveIndicator(status = '') {
        const indicator = document.getElementById('autoSaveIndicator');
        const statusEl = indicator.querySelector('.save-status');

        statusEl.className = 'save-status';

        if (status === 'saving') {
            statusEl.classList.add('saving');
            statusEl.textContent = 'Saving...';
        } else if (status === 'saved') {
            statusEl.classList.add('saved');
            const timeStr = this.state.lastSaved ? this.formatTime(this.state.lastSaved) : '';
            statusEl.textContent = `Saved ${timeStr}`;
        } else {
            statusEl.textContent = 'Auto-save enabled';
        }
    }

    formatTime(date) {
        const now = new Date();
        const diff = Math.floor((now - date) / 1000);

        if (diff < 60) return 'just now';
        if (diff < 3600) return `${Math.floor(diff / 60)}m ago`;
        if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;
        return date.toLocaleString();
    }

    showSuccessModal(referenceNumber) {
        const modal = document.getElementById('successModal');
        document.getElementById('referenceNumber').textContent = referenceNumber;
        modal.classList.add('show');
    }

    hideSuccessModal() {
        document.getElementById('successModal').classList.remove('show');
    }

    printSummary() {
        window.print();
    }

    resetForm() {
        this.state.clearStorage();
        this.state = new ApplicationState();
        this.hideSuccessModal();
        this.goToStep(1);
        this.populateFormFromState();
        document.getElementById('fileList').innerHTML = '';
    }

    showLoading() {
        document.getElementById('loadingOverlay').classList.add('show');
    }

    hideLoading() {
        document.getElementById('loadingOverlay').classList.remove('show');
    }

    showError(message) {
        alert(`Error: ${message}`);
    }

    showSuccess(message) {
        // Simple success notification
        const notification = document.createElement('div');
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: var(--success-color);
            color: white;
            padding: 1rem 1.5rem;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow-lg);
            z-index: 9999;
            animation: fadeIn 0.3s ease;
        `;
        notification.textContent = message;
        document.body.appendChild(notification);

        setTimeout(() => {
            notification.remove();
        }, 3000);
    }
}

// ==========================================================================
// Initialize Application
// ==========================================================================
let formController;

document.addEventListener('DOMContentLoaded', () => {
    formController = new FormController();
});

// Export for use in HTML onclick handlers
window.formController = formController;
