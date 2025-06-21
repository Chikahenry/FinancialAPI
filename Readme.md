 ### Solution File (FinancialTransactionsAPI.sln)
```
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "FinancialTransactionsAPI", "FinancialTransactionsAPI\FinancialTransactionsAPI.csproj", "{GUID1}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "FinancialTransactionsAPI.Tests", "FinancialTransactionsAPI.Tests\FinancialTransactionsAPI.Tests.csproj", "{GUID2}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{GUID1}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{GUID1}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{GUID1}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{GUID1}.Release|Any CPU.Build.0 = Release|Any CPU
		{GUID2}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{GUID2}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{GUID2}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{GUID2}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
EndGlobal
```

## 6. API Usage Examples

### Authentication Examples
```bash
# Register a new user
curl -X POST "http://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "newuser",
    "email": "newuser@example.com",
    "password": "password123"
  }'

# Login
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123"
  }'
```

### Transaction Examples
```bash
# Create transaction
curl -X POST "http://localhost:5000/api/transactions" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 250.50,
    "description": "Monthly grocery shopping",
    "type": 1,
    "category": "Food"
  }'

# Get transactions with filters
curl -X GET "http://localhost:5000/api/transactions?type=1&fromDate=2024-01-01&toDate=2024-12-31&page=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Features Implemented:

✅ **Complete REST API** with CRUD operations
✅ **JWT Authentication** with role-based access control
✅ **Entity Framework Core** with In-Memory database
✅ **Comprehensive filtering** (date range, type, amount, category)
✅ **Pagination** support
✅ **Input validation** with data annotations
✅ **Global exception handling** middleware
✅ **Swagger documentation** with JWT support
✅ **Unit tests** with xUnit, FluentAssertions, and Moq
✅ **Integration tests** with WebApplicationFactory
✅ **CI/CD pipeline** with GitHub Actions
✅ **Docker support** with multi-stage builds
✅ **Security features** (password hashing, JWT tokens)
✅ **Logging** and health checks
✅ **Production-ready** architecture

## Running the Application:

1. **Local Development:**
   ```bash
   dotnet run --project src/FinancialTransactionsAPI
   ```

2. **Run Tests:**
   ```bash
   dotnet test src/FinancialTransactionsAPI.Tests
   ```

3. **Docker:**
   ```bash
   docker-compose up --build
   ```

The API will be available at `http://localhost:5000` with Swagger UI at `/swagger`.