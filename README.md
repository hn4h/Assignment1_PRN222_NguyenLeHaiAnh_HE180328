# Assignment1_PRN222_NguyenLeHaiAnh_HE180328

## How to Run the Project

### Prerequisites
- .NET 8.0 SDK
- Docker Desktop (for Docker method)
- SQL Server (for local development method)

### Method 1: Using Docker (Recommended)

1. **Start the application using Docker Compose:**
   ```powershell
   docker-compose up -d
   ```
   This will:
   - Start a SQL Server container on port 1444
   - Initialize the database with the FUNewsManagement.sql script
   - Build and run the web application on port 5000

2. **Access the application:**
   - Open your browser and navigate to: `http://localhost:5000`

3. **Stop the application:**
   ```powershell
   docker-compose down
   ```

4. **Stop and remove all data (including database):**
   ```powershell
   docker-compose down -v
   ```

### Method 2: Local Development without Docker

1. **Setup SQL Server:**
   - Ensure SQL Server is running on your machine
   - Create a new database named `FUNewsManagement`
   - Execute the `FUNewsManagement.sql` script to initialize the database

2. **Update Connection String (if needed):**
   - Open `Presentation/appsettings.json`
   - Update the `DefaultConnection` to match your SQL Server configuration:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1444;Database=FUNewsManagement;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;Encrypt=False"
     }
     ```

3. **Build and Run the application:**
   ```powershell
   cd Presentation
   dotnet restore
   dotnet run
   ```

4. **Access the application:**
   - Open your browser and navigate to: `http://localhost:5092` (or the URL shown in the terminal)

### Method 3: Using Visual Studio

1. **Open the solution:**
   - Open `NguyenLeHaiAnh_HE180328_Assignment1.sln` in Visual Studio

2. **Setup Database:**
   - Ensure SQL Server is accessible
   - Update connection string in `Presentation/appsettings.json` if needed
   - Execute the `FUNewsManagement.sql` script

3. **Run the project:**
   - Set `Presentation` as the startup project
   - Press F5 or click the Run button

## Roles and Login Credentials for Testing

### Admin
- **Email:** admin@FUNewsManagementSystem.org
- **Password:** @@abc123@@

### Staff
- **Email:** IsabellaDavid@FUNewsManagement.org
- **Password:** @1

### Lecturer
- **Email:** MichaelCharlotte@FUNewsManagement.org
- **Password:** @1