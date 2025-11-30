# Multi-Step License Application System

A simple, well-structured web application for processing professional license applications with a focus on user experience and maintainable code - perfect for assignments and interviews.

## Features

### Multi-Step Wizard Interface
- ✅ 5-step wizard with clear progress indication
- ✅ Step validation before allowing navigation to next step
- ✅ Support for "Next/Previous" navigation and direct step jumping
- ✅ Visual progress indicator showing current step
- ✅ State maintained across all steps with ability to review before submission

### Dynamic Form Generation
- ✅ 3 different account types: Individual, Business, Government
- ✅ Dynamically rendered form fields based on selected account type
- ✅ Conditional field visibility based on user selections
- ✅ Support for various field types: text, email, phone, dropdown, radio, checkbox, date picker, file upload

### Comprehensive Validation
- ✅ **Client-side validation**: Real-time validation with immediate feedback
- ✅ **Server-side validation**: DataAnnotations validation in DTOs
- ✅ Field-level error messages with clear indicators
- ✅ Business rule validation (e.g., date ranges, conditional requirements)

### File Upload
- ✅ Drag-and-drop file upload zone with visual feedback
- ✅ Support for multiple file uploads
- ✅ File type validation (PDF, JPG, PNG, DOCX)
- ✅ File size limits (10MB max)
- ✅ Visual file list with ability to remove files
- ✅ File metadata storage

### Draft Management & Auto-Save
- ✅ "Save as Draft" functionality accessible from any step
- ✅ Auto-save mechanism every 60 seconds
- ✅ Load previously saved drafts from localStorage
- ✅ Visual indicator showing last saved timestamp

### Indian Localization
- ✅ 36 Indian states and union territories
- ✅ 6-digit PIN code validation
- ✅ Default country set to India
- ✅ Indian field labels (GST/PAN, Aadhaar)

### Responsive Design
- ✅ Mobile-first approach (320px - 1920px+ viewport)
- ✅ Optimized layouts for mobile, tablet, and desktop
- ✅ Touch-friendly UI elements
- ✅ Print-friendly summary page

## Technology Stack

### Backend
- **Framework**: ASP.NET Core Web API (.NET 10.0)
- **Architecture**: Simple 3-Tier (Controllers → Services → Data)
- **Validation**: DataAnnotations (built-in)
- **Database**: Entity Framework Core with In-Memory Database
- **File Storage**: Local file system storage
- **Logging**: Serilog with console and file outputs

### Frontend
- **Core Technologies**: HTML5, CSS3, Vanilla JavaScript (ES6+)
- **Styling**: Custom CSS with CSS Grid/Flexbox (Mobile-first)
- **State Management**: JavaScript class-based state management with localStorage
- **HTTP Client**: Fetch API
- **No Frameworks**: Pure JavaScript for simplicity

## Project Structure

```
Multi-Step-Acc-Appl/
├── backend/
│   └── LicenseApplication/              # Single project - Simple architecture
│       ├── Controllers/                 # API endpoints
│       │   ├── ApplicationsController.cs
│       │   ├── FilesController.cs
│       │   └── AccountTypesController.cs
│       ├── Services/                    # Business logic
│       │   ├── ApplicationService.cs
│       │   └── FileStorageService.cs
│       ├── Models/                      # Database entities
│       │   ├── Application.cs
│       │   ├── FileMetadata.cs
│       │   ├── AccountType.cs
│       │   └── ApplicationStatus.cs
│       ├── DTOs/                        # Data Transfer Objects
│       │   ├── CreateApplicationDto.cs
│       │   ├── UpdateDraftDto.cs
│       │   ├── ApplicationDto.cs
│       │   └── FileMetadataDto.cs
│       ├── Data/                        # Database context
│       │   └── ApplicationDbContext.cs
│       ├── wwwroot/                     # Frontend files
│       │   ├── index.html
│       │   ├── css/
│       │   │   └── styles.css
│       │   └── js/
│       │       └── app.js
│       └── Program.cs                   # Application startup
├── SIMPLIFIED_ARCHITECTURE.md           # Architecture documentation
└── README.md
```

## Architecture

### Simple 3-Tier Architecture

```
┌─────────────────────────────────────┐
│       Frontend (wwwroot)            │
│    HTML + CSS + JavaScript          │
└─────────────┬───────────────────────┘
              │ HTTP/JSON
┌─────────────▼───────────────────────┐
│       Controllers (API Layer)       │
│  - ApplicationsController           │
│  - FilesController                  │
│  - AccountTypesController           │
└─────────────┬───────────────────────┘
              │ Calls
┌─────────────▼───────────────────────┐
│       Services (Business Logic)     │
│  - ApplicationService               │
│  - FileStorageService               │
└─────────────┬───────────────────────┘
              │ Uses
┌─────────────▼───────────────────────┐
│  Data Layer (EF Core + In-Memory)   │
│  - ApplicationDbContext             │
│  - Models (Application, FileData)   │
└─────────────────────────────────────┘
```

**Why This Architecture?**
- **Simple**: Easy to understand and explain in interviews
- **Organized**: Clear separation of concerns
- **Appropriate**: Right level of complexity for the task
- **Maintainable**: Straightforward to modify and extend

## Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later
- Any modern web browser (Chrome, Firefox, Edge, Safari)

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd Multi-Step-Acc-Appl
```

2. Navigate to the backend directory:
```bash
cd backend/LicenseApplication
```

### Running the Application

1. Run the application:
```bash
dotnet run
```

2. Open your browser and navigate to:
   - Application: `http://localhost:5157`

The application will:
- Serve the frontend from `wwwroot`
- Initialize an in-memory database
- Create the `uploads` folder for file storage
- Start logging to `logs/` directory

**That's it!** No complex setup, no migrations, just run and use.

## Usage Guide

### Filling Out an Application

1. **Step 1: Account Type**
   - Select an account type (Individual, Business, or Government)
   - Each type has different fields

2. **Step 2: Account Details**
   - Fill in the details specific to your account type
   - Fields change dynamically based on your selection

3. **Step 3: Address Information**
   - Enter your complete address
   - Select state from 36 Indian states/UTs
   - Enter 6-digit PIN code

4. **Step 4: License Information**
   - Indicate if you have a previous license
   - Upload supporting documents (drag-and-drop or browse)
   - Supported formats: PDF, JPG, PNG, DOCX (max 10MB)

5. **Step 5: Review & Submit**
   - Review all entered information
   - Agree to terms and conditions
   - Add optional notes
   - Submit application

### Additional Features

- **Save as Draft**: Click "Save as Draft" to save your progress
- **Auto-save**: Automatic save every 60 seconds
- **Navigation**: Use Next/Previous or click on step indicators
- **Validation**: Real-time error messages
- **Print**: Print summary after submission

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/applications` | Submit application |
| POST | `/api/applications/draft` | Create draft |
| PUT | `/api/applications/{id}/draft` | Update draft |
| GET | `/api/applications/{id}` | Get by ID |
| GET | `/api/applications/reference/{ref}` | Get by reference number |
| POST | `/api/applications/validate` | Validate data |
| POST | `/api/files/upload` | Upload file |
| DELETE | `/api/files/{id}` | Delete file |
| GET | `/api/accounttypes` | Get account types |

### Example: Submit Application

```bash
curl -X POST http://localhost:5157/api/applications \
  -H "Content-Type: application/json" \
  -d '{
    "accountType": 1,
    "accountName": "John Doe",
    "email": "john@example.com",
    "phone": "9876543210",
    "country": "India",
    "zipCode": "110001"
  }'
```

## Configuration

### Frontend Configuration

Edit `wwwroot/js/app.js`:

```javascript
const CONFIG = {
    API_BASE_URL: window.location.origin + '/api',
    AUTO_SAVE_INTERVAL: 60000, // 60 seconds
    MAX_FILE_SIZE: 10 * 1024 * 1024, // 10MB
    ALLOWED_FILE_TYPES: ['.pdf', '.jpg', '.jpeg', '.png', '.docx', '.doc']
};
```

### Backend Configuration

Edit `Program.cs` for:
- CORS origins
- Database (switch to SQL Server for production)
- File storage path
- Logging settings

## Validation Rules

### Common Fields
- **Email**: Valid email format
- **Phone**: 10 digits (Indian format)
- **PIN Code**: 6 digits (Indian format)

### Account Type Specific

**Individual:**
- First Name, Last Name: Required
- Date of Birth: 18+ years old
- Aadhaar: Optional

**Business:**
- Business Name: Required
- GST/PAN Number: Required
- Established Date: Cannot be in future

**Government:**
- Agency Name, Department: Required
- Authorized Officer: Required
- Government ID: Required

## Troubleshooting

### Common Issues

**Application won't start:**
```bash
# Kill existing processes
powershell "Get-Process -Name dotnet -ErrorAction SilentlyContinue | Stop-Process -Force"

# Restart
dotnet run
```

**Browser cache issues:**
- Press `Ctrl + Shift + R` (hard refresh)
- Or clear browser cache completely

**Port already in use:**
- Application uses port 5157 by default
- Change in `Properties/launchSettings.json` if needed

**File upload fails:**
- Check file size (max 10MB)
- Verify file type is allowed
- Ensure `uploads` directory exists

## Database

**Current:** In-Memory Database (data lost on restart)

**Switch to SQL Server:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

Then run:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Performance Features

- ✅ In-memory database for fast development
- ✅ Client-side state management reduces server calls
- ✅ Auto-save debouncing prevents excessive API calls
- ✅ Minimal JavaScript bundle (no frameworks)
- ✅ CSS animations use GPU acceleration

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Testing

**Manual Testing:**
1. Fill out form with different account types
2. Test validation with invalid data
3. Try file upload with various file types
4. Test save as draft
5. Submit and verify success modal

**API Testing:**
```bash
# Test account types endpoint
curl http://localhost:5157/api/accounttypes

# Test create application
curl -X POST http://localhost:5157/api/applications \
  -H "Content-Type: application/json" \
  -d '{"accountType":1,"accountName":"Test","email":"test@example.com"}'
```


---

**Built with ASP.NET Core and Vanilla JavaScript**
*Simple, Clean, and Interview-Ready* ✨
