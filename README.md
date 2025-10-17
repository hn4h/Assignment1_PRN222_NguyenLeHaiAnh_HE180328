# Assignment1_PRN222_NguyenLeHaiAnh_HE180328

## How to Run the Project

### Prerequisites
- Docker Desktop

### Using Docker (Recommended)

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