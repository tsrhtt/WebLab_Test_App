using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApi.Data;
using MyApi.DTOs;
using MyApi.Models;

namespace MyApi.Services
{
    public class WebLabService
    {
        private readonly HttpClient _client;
        private string? _token;
        private readonly AppDbContext _dbContext;

        public WebLabService(IConfiguration configuration, AppDbContext dbContext)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://public.ehealth.by/lab-test/api/integration/")
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _dbContext = dbContext;
        }

        public async Task InitializeAsync(IConfiguration configuration)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://public.ehealth.by/lab-test/keycloak/realms/laboratory/protocol/openid-connect/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"client_id", configuration["Keycloak:ClientId"]},
                    {"client_secret", configuration["Keycloak:ClientSecret"]},
                    {"grant_type", "client_credentials"}
                })
            };
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);
            _token = tokenResponse?.AccessToken;
            Console.WriteLine($"Access token: {_token}");
        }

        public async Task<IEnumerable<DirectionDto>> GetLabData()
        {
            if (_token == null) throw new InvalidOperationException("Service not initialized with token.");
            Console.WriteLine($"Using token: {_token} for data fetch.");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.GetAsync("Direction/");
            Console.WriteLine($"HTTP GET request sent to {_client.BaseAddress}Direction/");

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Request failed with status code {response.StatusCode}: {responseContent}");
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}, Content: {responseContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("JSON Response:");
            Console.WriteLine(content);

            try
            {
                var responseData = JsonSerializer.Deserialize<ResponseWrapper>(content, new JsonSerializerOptions
                {
                    Converters = { new DateTimeConverterHandlingUTC() }
                });

                Console.WriteLine("Deserialization check:");
                foreach (var direction in responseData?.Data ?? Enumerable.Empty<Direction>())
                {
                    Console.WriteLine($"ID: {direction.Id}, StatusHistories: {direction.DirectionStatusHistory?.Count}, IndicatorGroups: {direction.IndicatorGroups?.Count}");
                    if (direction.DirectionStatusHistory != null)
                    {
                        foreach (var statusHistory in direction.DirectionStatusHistory)
                        {
                            Console.WriteLine($"StatusHistory ID: {statusHistory.Id}, DateTime: {statusHistory.DateTime}, DirectionStatusId: {statusHistory.DirectionStatusId}, UserFio: {statusHistory.UserFio}, Comment: {statusHistory.Comment}");
                        }
                    }
                    if (direction.IndicatorGroups != null)
                    {
                        foreach (var indicatorGroup in direction.IndicatorGroups)
                        {
                            Console.WriteLine($"IndicatorGroup Key: {indicatorGroup.Key}, Indicators Count: {indicatorGroup.Value?.Count}");
                            if (indicatorGroup.Value != null)
                            {
                                foreach (var indicator in indicatorGroup.Value)
                                {
                                    Console.WriteLine($"Indicator ID: {indicator.Id}, Name: {indicator.Name}");
                                }
                            }
                        }
                    }
                }

                if (responseData?.Data != null)
                {
                    foreach (var direction in responseData.Data)
                    {
                        if (direction != null)
                        {
                            Console.WriteLine($"Deserialized - ID: {direction.Id}, StatusHistories: {direction.DirectionStatusHistory?.Count}, IndicatorGroups: {direction.IndicatorGroups?.Count}");
                            await HandleDirection(direction);
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                }

                var directions = responseData?.Data.Select(d => new DirectionDto
                {
                    Id = d.Id,
                    PatientId = d.PatientId,
                    Patient = new PatientDto
                    {
                        Id = d.Patient.Id,
                        IdentificationNumber = d.Patient.IdentificationNumber,
                        PatientId = d.Patient.PatientId,
                        Name = d.Patient.Name,
                        Surname = d.Patient.Surname,
                        SecondName = d.Patient.SecondName,
                        FullName = d.Patient.FullName,
                        Sex = d.Patient.Sex,
                        BirthDate = d.Patient.BirthDate,
                        SexDescription = d.Patient.SexDescription,
                        Age = d.Patient.Age
                    },
                    LaboratoryId = d.LaboratoryId,
                    Laboratory = d.Laboratory,
                    AnalysTypeId = d.AnalysTypeId,
                    AnalysTypeName = d.AnalysTypeName,
                    AnalysTypeFormat = d.AnalysTypeFormat,
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    DirectionStatusHistory = d.DirectionStatusHistory?.Select(sh => new DirectionStatusHistoryDto
                    {
                        Id = sh.Id,
                        DirectionId = sh.DirectionId,
                        DateTime = sh.DateTime,
                        DirectionStatusId = sh.DirectionStatusId,
                        UserFio = sh.UserFio,
                        Comment = sh.Comment
                    }).ToList(),
                    Indicators = d.IndicatorGroups?.SelectMany(ig => ig.Value).Select(i => new IndicatorDto
                    {
                        Id = i.Id,
                        DirectionId = d.Id, // Ensure the correct DirectionId is set
                        Name = i.Name,
                        Abbreviation = i.Abbreviation,
                        Units = i.Units,
                        Type = i.Type,
                        Comment = i.Comment,
                        IsAdditional = i.IsAdditional,
                        IsNormExist = i.IsNormExist,
                        IsInReference = i.IsInReference,
                        Group = i.Group,
                        GroupOrderNumber = i.GroupOrderNumber,
                        SortNumber = i.SortNumber,
                        MinStandardValue = i.MinStandardValue,
                        MaxStandardValue = i.MaxStandardValue,
                        ResultVal = i.ResultVal,
                        ResultStr = i.ResultStr,
                        TextStandards = i.TextStandards,
                        PossibleStringValues = i.PossibleStringValues,
                        DynamicValues = i.DynamicValues
                    }).ToList(),
                    PatientFullName = d.PatientFullName,
                    Cito = d.Cito,
                    IsArchived = d.IsArchived,
                    DirectionStatus = d.DirectionStatus,
                    DirectionStatusId = d.DirectionStatusId,
                    Category = d.Category,
                    RequestDate = d.RequestDate,
                    RequestedBy = d.RequestedBy,
                    AcceptedDate = d.AcceptedDate,
                    AcceptedBy = d.AcceptedBy,
                    OnDate = d.OnDate,
                    ReadyDate = d.ReadyDate,
                    Sid = d.Sid,
                    HasAnyResults = d.HasAnyResults,
                    LaborantComment = d.LaborantComment,
                    SamplingDate = d.SamplingDate,
                    SamplingDateStr = d.SamplingDateStr,
                    SampleNumber = d.SampleNumber,
                    SamplingDoctorFio = d.SamplingDoctorFio,
                    DoctorLabDiagnosticFio = d.DoctorLabDiagnosticFio,
                    DoctorFeldsherLaborantFio = d.DoctorFeldsherLaborantFio,
                    DoctorBiologFio = d.DoctorBiologFio,
                    BioMaterialCount = d.BioMaterialCount,
                    BioMaterialType = d.BioMaterialType,
                    NumberByJournal = d.NumberByJournal
                }).ToList();

                return directions;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JsonException encountered: {ex.Message}");
                throw;
            }
        }

        public async Task<DirectionDto> GetDetailedDirection(int directionId)
        {
            if (_token == null) throw new InvalidOperationException("Service not initialized with token.");
            Console.WriteLine($"Using token: {_token} for detailed data fetch.");

            // Fetch detailed data including indicators from the external source
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.GetAsync($"Direction/{directionId}");
            Console.WriteLine($"HTTP GET request sent to {_client.BaseAddress}Direction/{directionId}");

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Request failed with status code {response.StatusCode}: {responseContent}");
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}, Content: {responseContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("JSON Response:");
            Console.WriteLine(content);

            Direction externalDirection;
            try
            {
                externalDirection = JsonSerializer.Deserialize<Direction>(content, new JsonSerializerOptions
                {
                    Converters = { new DateTimeConverterHandlingUTC() }
                });

                if (externalDirection == null)
                {
                    throw new InvalidOperationException("Failed to deserialize external direction data.");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JsonException encountered: {ex.Message}");
                throw;
            }

            // Fetch direction details from the database (excluding indicators)
            var dbDirection = await _dbContext.Directions
                .Include(d => d.Patient)
                .Include(d => d.LaboratoryData)
                .Include(d => d.AnalysType)
                .Include(d => d.DirectionStatusHistory)
                .FirstOrDefaultAsync(d => d.Id == directionId);

            if (dbDirection == null)
            {
                throw new InvalidOperationException("Direction not found.");
            }

            // Map database entity to DTO with null checks
            var directionDto = new DirectionDto
            {
                Id = dbDirection.Id,
                PatientId = dbDirection.PatientId,
                Patient = dbDirection.Patient != null ? new PatientDto
                {
                    Id = dbDirection.Patient.Id,
                    IdentificationNumber = dbDirection.Patient.IdentificationNumber,
                    PatientId = dbDirection.Patient.PatientId,
                    Name = dbDirection.Patient.Name,
                    Surname = dbDirection.Patient.Surname,
                    SecondName = dbDirection.Patient.SecondName,
                    FullName = dbDirection.Patient.FullName,
                    Sex = dbDirection.Patient.Sex,
                    BirthDate = dbDirection.Patient.BirthDate,
                    SexDescription = dbDirection.Patient.SexDescription,
                    Age = dbDirection.Patient.Age
                } : null,
                LaboratoryId = dbDirection.LaboratoryId,
                Laboratory = dbDirection.LaboratoryData?.Name,
                AnalysTypeId = dbDirection.AnalysTypeId,
                AnalysTypeName = dbDirection.AnalysType?.Name,
                AnalysTypeFormat = dbDirection.AnalysType.Format,
                DepartmentId = dbDirection.DepartmentId,
                DepartmentName = dbDirection.Department?.Name,
                DirectionStatusHistory = dbDirection.DirectionStatusHistory?.Select(sh => new DirectionStatusHistoryDto
                {
                    Id = sh.Id,
                    DirectionId = sh.DirectionId,
                    DateTime = sh.DateTime,
                    DirectionStatusId = sh.DirectionStatusId,
                    UserFio = sh.UserFio,
                    Comment = sh.Comment
                }).ToList(),
                PatientFullName = dbDirection.PatientFullName,
                Cito = dbDirection.Cito,
                IsArchived = dbDirection.IsArchived,
                DirectionStatus = dbDirection.DirectionStatus,
                DirectionStatusId = dbDirection.DirectionStatusId,
                Category = dbDirection.Category,
                RequestDate = dbDirection.RequestDate,
                RequestedBy = dbDirection.RequestedBy,
                AcceptedDate = dbDirection.AcceptedDate,
                AcceptedBy = dbDirection.AcceptedBy,
                OnDate = dbDirection.OnDate,
                ReadyDate = dbDirection.ReadyDate,
                Sid = dbDirection.Sid,
                HasAnyResults = dbDirection.HasAnyResults,
                LaborantComment = dbDirection.LaborantComment,
                SamplingDate = dbDirection.SamplingDate,
                SamplingDateStr = dbDirection.SamplingDateStr,
                SampleNumber = dbDirection.SampleNumber,
                SamplingDoctorFio = dbDirection.SamplingDoctorFio,
                DoctorLabDiagnosticFio = dbDirection.DoctorLabDiagnosticFio,
                DoctorFeldsherLaborantFio = dbDirection.DoctorFeldsherLaborantFio,
                DoctorBiologFio = dbDirection.DoctorBiologFio,
                BioMaterialCount = dbDirection.BioMaterialCount,
                BioMaterialType = dbDirection.BioMaterialType,
                NumberByJournal = dbDirection.NumberByJournal
            };

            // Add indicators from the external source to the DTO
            directionDto.Indicators = externalDirection.IndicatorGroups?.SelectMany(ig => ig.Value).Select(i => new IndicatorDto
            {
                Id = i.Id,
                DirectionId = directionId,
                Name = i.Name,
                Abbreviation = i.Abbreviation,
                Units = i.Units,
                Type = i.Type,
                Comment = i.Comment,
                IsAdditional = i.IsAdditional,
                IsNormExist = i.IsNormExist,
                IsInReference = i.IsInReference,
                Group = i.Group,
                GroupOrderNumber = i.GroupOrderNumber,
                SortNumber = i.SortNumber,
                MinStandardValue = i.MinStandardValue,
                MaxStandardValue = i.MaxStandardValue,
                ResultVal = i.ResultVal,
                ResultStr = i.ResultStr,
                TextStandards = i.TextStandards,
                PossibleStringValues = i.PossibleStringValues,
                DynamicValues = i.DynamicValues
            }).ToList();

            return directionDto;
        }



        public async Task<IEnumerable<DirectionDto>> GetDirectionsFromDb()
        {
            // Clear the indicators table before fetching directions
            await ClearIndicatorsTable();

            var directions = await _dbContext.Directions
                .Include(d => d.Patient)
                .Include(d => d.LaboratoryData)
                .Include(d => d.AnalysType)
                .Include(d => d.DirectionStatusHistory)
                .Select(d => new DirectionDto
                {
                    Id = d.Id,
                    PatientId = d.PatientId,
                    Patient = new PatientDto
                    {
                        Id = d.Patient.Id,
                        IdentificationNumber = d.Patient.IdentificationNumber,
                        PatientId = d.Patient.PatientId,
                        Name = d.Patient.Name,
                        Surname = d.Patient.Surname,
                        SecondName = d.Patient.SecondName,
                        FullName = d.Patient.FullName,
                        Sex = d.Patient.Sex,
                        BirthDate = d.Patient.BirthDate,
                        SexDescription = d.Patient.SexDescription,
                        Age = d.Patient.Age
                    },
                    LaboratoryId = d.LaboratoryId,
                    Laboratory = d.LaboratoryData.Name,
                    AnalysTypeId = d.AnalysTypeId,
                    AnalysTypeName = d.AnalysType.Name,
                    AnalysTypeFormat = d.AnalysType.Format,
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.Department.Name,
                    DirectionStatusHistory = d.DirectionStatusHistory.Select(sh => new DirectionStatusHistoryDto
                    {
                        Id = sh.Id,
                        DirectionId = sh.DirectionId,
                        DateTime = sh.DateTime,
                        DirectionStatusId = sh.DirectionStatusId,
                        UserFio = sh.UserFio,
                        Comment = sh.Comment
                    }).ToList(),
                    PatientFullName = d.PatientFullName,
                    Cito = d.Cito,
                    IsArchived = d.IsArchived,
                    DirectionStatus = d.DirectionStatus,
                    DirectionStatusId = d.DirectionStatusId,
                    Category = d.Category,
                    RequestDate = d.RequestDate,
                    RequestedBy = d.RequestedBy,
                    AcceptedDate = d.AcceptedDate,
                    AcceptedBy = d.AcceptedBy,
                    OnDate = d.OnDate,
                    ReadyDate = d.ReadyDate,
                    Sid = d.Sid,
                    HasAnyResults = d.HasAnyResults,
                    LaborantComment = d.LaborantComment,
                    SamplingDate = d.SamplingDate,
                    SamplingDateStr = d.SamplingDateStr,
                    SampleNumber = d.SampleNumber,
                    SamplingDoctorFio = d.SamplingDoctorFio,
                    DoctorLabDiagnosticFio = d.DoctorLabDiagnosticFio,
                    DoctorFeldsherLaborantFio = d.DoctorFeldsherLaborantFio,
                    DoctorBiologFio = d.DoctorBiologFio,
                    BioMaterialCount = d.BioMaterialCount,
                    BioMaterialType = d.BioMaterialType,
                    NumberByJournal = d.NumberByJournal
                }).ToListAsync();

            return directions;
        }

        private async Task ClearIndicatorsTable()
        {
            _dbContext.Indicators.RemoveRange(_dbContext.Indicators);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AcceptDirection(int directionId, string acceptedBy, string comment)
        {
            var existingDirection = await _dbContext.Directions
                .Include(d => d.DirectionStatusHistory)
                .FirstOrDefaultAsync(d => d.Id == directionId);

            if (existingDirection == null)
            {
                throw new InvalidOperationException("Direction not found.");
            }

            var currentDate = DateTime.UtcNow;

            if (!existingDirection.BioMaterialCount.HasValue)
            {
                existingDirection.DirectionStatusId = 3; // Нет биоматериала
                existingDirection.DirectionStatus = "Нет биоматериала";
                existingDirection.AcceptedDate = null;
                existingDirection.AcceptedBy = null;
            }
            else
            {
                existingDirection.DirectionStatusId = 6; // В работе
                existingDirection.DirectionStatus = "В работе";
                existingDirection.AcceptedDate = currentDate;
                existingDirection.AcceptedBy = acceptedBy;
            }

            var newHistory = new DirectionStatusHistory
            {
                Id = await _dbContext.DirectionStatusHistories.MaxAsync(h => (int?)h.Id) + 1 ?? 1,
                DirectionId = existingDirection.Id,
                DateTime = currentDate,
                DirectionStatusId = existingDirection.DirectionStatusId,
                UserFio = existingDirection.DirectionStatusId == 3 ? null : acceptedBy, // Set to null if status is 3
                Comment = comment
            };

            _dbContext.DirectionStatusHistories.Add(newHistory);

            await _dbContext.SaveChangesAsync();
        }


        public async Task UpdateDirection(DirectionDto updatedDirectionDto)
        {
            var existingDirection = await _dbContext.Directions
                .Include(d => d.Patient)
                .Include(d => d.LaboratoryData)
                .Include(d => d.AnalysType)
                .Include(d => d.DirectionStatusHistory)
                .Include(d => d.Indicators)
                .FirstOrDefaultAsync(d => d.Id == updatedDirectionDto.Id);

            if (existingDirection == null)
            {
                throw new InvalidOperationException("Direction not found.");
            }

            // Update patient information
            existingDirection.Patient.FullName = updatedDirectionDto.Patient.FullName;
            existingDirection.Patient.SexDescription = updatedDirectionDto.Patient.SexDescription;
            existingDirection.Patient.Sex = updatedDirectionDto.Patient.SexDescription == "Мужской" ? 1 : 2;
            existingDirection.Patient.BirthDate = updatedDirectionDto.Patient.BirthDate.ToUniversalTime();
            existingDirection.Patient.IdentificationNumber = updatedDirectionDto.Patient.IdentificationNumber;

            // Update other fields
            existingDirection.Laboratory = updatedDirectionDto.Laboratory;
            existingDirection.DepartmentName = updatedDirectionDto.DepartmentName;
            existingDirection.PatientFullName = updatedDirectionDto.PatientFullName;
            existingDirection.RequestedBy = updatedDirectionDto.RequestedBy;
            existingDirection.LaborantComment = updatedDirectionDto.LaborantComment;
            existingDirection.OnDate = updatedDirectionDto.OnDate?.ToUniversalTime();
            existingDirection.Cito = updatedDirectionDto.Cito;
            existingDirection.SamplingDate = updatedDirectionDto.SamplingDate?.ToUniversalTime();
            existingDirection.SampleNumber = updatedDirectionDto.SampleNumber;
            existingDirection.SamplingDoctorFio = updatedDirectionDto.SamplingDoctorFio;
            existingDirection.BioMaterialCount = updatedDirectionDto.BioMaterialCount;
            existingDirection.BioMaterialType = updatedDirectionDto.BioMaterialType;

            // Update related IDs if necessary
            var labData = await _dbContext.LaboratoryDatas.FirstOrDefaultAsync(l => l.Name == updatedDirectionDto.Laboratory);
            if (labData != null)
            {
                existingDirection.LaboratoryId = labData.Id;
            }
            else
            {
                labData = new LaboratoryData { Name = updatedDirectionDto.Laboratory };
                _dbContext.LaboratoryDatas.Add(labData);
                await _dbContext.SaveChangesAsync();
                existingDirection.LaboratoryId = labData.Id;
            }

            var department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Name == updatedDirectionDto.DepartmentName);
            if (department != null)
            {
                existingDirection.DepartmentId = department.Id;
            }
            else
            {
                department = new Department { Name = updatedDirectionDto.DepartmentName };
                _dbContext.Departments.Add(department);
                await _dbContext.SaveChangesAsync();
                existingDirection.DepartmentId = department.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteDirection(int directionId)
        {
            var direction = await _dbContext.Directions
                .Include(d => d.DirectionStatusHistory)
                .Include(d => d.Indicators)
                .FirstOrDefaultAsync(d => d.Id == directionId);

            if (direction == null)
            {
                throw new InvalidOperationException("Direction not found.");
            }

            _dbContext.Directions.Remove(direction);
            await _dbContext.SaveChangesAsync();
        }

        private async Task HandleDirection(Direction direction)
        {
            if (direction == null)
            {
                Console.WriteLine("Direction is null");
                return;
            }

            Console.WriteLine($"Processing Direction ID: {direction.Id}, Patient ID: {direction.Patient?.PatientId}, Laboratory ID: {direction.LaboratoryId}");

            direction.DirectionStatusHistory = direction.DirectionStatusHistory ?? new List<DirectionStatusHistory>();
            direction.Indicators = direction.IndicatorGroups?.SelectMany(ig => ig.Value).ToList() ?? new List<Indicator>();

            Console.WriteLine("Initial DirectionStatusHistory count: " + direction.DirectionStatusHistory.Count);
            Console.WriteLine("Initial Indicators count: " + direction.Indicators.Count);

            foreach (var statusHistory in direction.DirectionStatusHistory)
            {
                Console.WriteLine($"Before Saving - StatusHistory ID: {statusHistory.Id}, DateTime: {statusHistory.DateTime}, DirectionStatusId: {statusHistory.DirectionStatusId}, UserFio: {statusHistory.UserFio}, Comment: {statusHistory.Comment}");
            }

            var existingPatient = await _dbContext.Patients.FindAsync(direction.Patient.Id);
            if (existingPatient == null)
            {
                Console.WriteLine("Patient not found, adding new");
                _dbContext.Patients.Add(direction.Patient);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Patient already exists, updating existing patient");
                _dbContext.Entry(existingPatient).CurrentValues.SetValues(direction.Patient);
                direction.Patient = existingPatient;
            }

            direction.PatientId = direction.Patient.Id;

            var dbLaboratory = await _dbContext.LaboratoryDatas.FindAsync(direction.LaboratoryId);
            if (dbLaboratory == null)
            {
                Console.WriteLine("Laboratory not found, adding new");
                dbLaboratory = new LaboratoryData { Id = direction.LaboratoryId, Name = direction.Laboratory };
                _dbContext.LaboratoryDatas.Add(dbLaboratory);
            }
            else
            {
                dbLaboratory.Name = direction.Laboratory;
                _dbContext.Entry(dbLaboratory).CurrentValues.SetValues(dbLaboratory);
            }

            var dbAnalysType = await _dbContext.AnalysTypes.FindAsync(direction.AnalysTypeId);
            if (dbAnalysType == null)
            {
                Console.WriteLine("AnalysType not found, adding new");
                dbAnalysType = new AnalysType
                {
                    Id = direction.AnalysTypeId,
                    Name = direction.AnalysTypeName,
                    Format = direction.AnalysTypeFormat
                };
                _dbContext.AnalysTypes.Add(dbAnalysType);
            }
            else
            {
                dbAnalysType.Name = direction.AnalysTypeName;
                dbAnalysType.Format = direction.AnalysTypeFormat;
                _dbContext.Entry(dbAnalysType).CurrentValues.SetValues(dbAnalysType);
            }

            Console.WriteLine($"Adding/updating AnalysisType with ID {direction.AnalysTypeId}, Name {direction.AnalysTypeName}, Format {direction.AnalysTypeFormat}");

            if (direction.DepartmentId.HasValue)
            {
                var dbDepartment = await _dbContext.Departments.FindAsync(direction.DepartmentId);
                if (dbDepartment == null)
                {
                    Console.WriteLine("Department not found, adding new if not null");
                    if (!string.IsNullOrEmpty(direction.DepartmentName))
                    {
                        dbDepartment = new Department { Id = direction.DepartmentId.Value, Name = direction.DepartmentName };
                        _dbContext.Departments.Add(dbDepartment);
                    }
                }
                else
                {
                    dbDepartment.Name = direction.DepartmentName;
                    _dbContext.Entry(dbDepartment).CurrentValues.SetValues(dbDepartment);
                }
            }

            Console.WriteLine("Handling the Direction entity...");
            var existingDirection = await _dbContext.Directions
                .Include(d => d.DirectionStatusHistory)
                .Include(d => d.Indicators)
                .FirstOrDefaultAsync(d => d.Id == direction.Id);

            if (existingDirection == null)
            {
                Console.WriteLine("Direction not found, adding new");
                _dbContext.Directions.Add(direction);
            }
            else
            {
                Console.WriteLine("Updating existing Direction");
                _dbContext.Entry(existingDirection).CurrentValues.SetValues(direction);

                existingDirection.DirectionStatusHistory.Clear();
                foreach (var statusHistory in direction.DirectionStatusHistory)
                {
                    Console.WriteLine($"Adding DirectionStatusHistory ID: {statusHistory.Id}");
                    existingDirection.DirectionStatusHistory.Add(new DirectionStatusHistory
                    {
                        Id = statusHistory.Id,
                        DirectionId = statusHistory.DirectionId,
                        DateTime = statusHistory.DateTime,
                        DirectionStatusId = statusHistory.DirectionStatusId,
                        UserFio = statusHistory.UserFio,
                        Comment = statusHistory.Comment
                    });
                }

                // Clear existing indicators only if they belong to the current direction
                var existingIndicators = await _dbContext.Indicators
                    .Where(i => i.DirectionId == existingDirection.Id)
                    .ToListAsync();
                _dbContext.Indicators.RemoveRange(existingIndicators);

                foreach (var indicator in direction.Indicators)
                {
                    Console.WriteLine($"Adding Indicator ID: {indicator.Id}");
                    indicator.DirectionId = existingDirection.Id; // Ensure DirectionId is correctly set

                    var existingIndicator = await _dbContext.Indicators
                        .AsNoTracking()
                        .FirstOrDefaultAsync(i => i.Id == indicator.Id && i.DirectionId == indicator.DirectionId);

                    if (existingIndicator == null)
                    {
                        _dbContext.Indicators.Add(indicator);
                    }
                    else
                    {
                        _dbContext.Entry(indicator).State = EntityState.Modified;
                    }
                }
            }

            try
            {
                Console.WriteLine("Saving changes to database...");
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Concurrency exception occurred while saving changes:");
                Console.WriteLine(ex.Message);

                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Direction dir)
                    {
                        Console.WriteLine($"Concurrency conflict on Direction ID: {dir.Id}");
                        entry.Reload();
                    }
                    else if (entry.Entity is Indicator ind)
                    {
                        Console.WriteLine($"Concurrency conflict on Indicator ID: {ind.Id}");
                        entry.Reload();
                    }
                    else if (entry.Entity is DirectionStatusHistory dsh)
                    {
                        Console.WriteLine($"Concurrency conflict on DirectionStatusHistory ID: {dsh.Id}");
                        entry.Reload();
                    }
                    else if (entry.Entity is Patient pat)
                    {
                        Console.WriteLine($"Concurrency conflict on Patient ID: {pat.Id}");
                        entry.Reload();
                    }
                    else if (entry.Entity is LaboratoryData labData)
                    {
                        Console.WriteLine($"Concurrency conflict on LaboratoryData ID: {labData.Id}");
                        entry.Reload();
                    }
                    else if (entry.Entity is AnalysType analysType)
                    {
                        Console.WriteLine($"Concurrency conflict on AnalysType ID: {analysType.Id}");
                        entry.Reload();
                    }
                    else if (entry.Entity is Department dept)
                    {
                        Console.WriteLine($"Concurrency conflict on Department ID: {dept.Id}");
                        entry.Reload();
                    }
                    else
                    {
                        Console.WriteLine("Concurrency conflict on unknown entity type");
                        throw new NotSupportedException(
                            "A concurrency conflict was detected for an unknown entity type.");
                    }
                }

                // Now that the entities have been refreshed, try to save again
                await _dbContext.SaveChangesAsync();
            }

            var savedDirection = await _dbContext.Directions
                .Include(d => d.DirectionStatusHistory)
                .Include(d => d.Indicators)
                .FirstOrDefaultAsync(d => d.Id == direction.Id);

            Console.WriteLine("Saved DirectionStatusHistory count: " + savedDirection.DirectionStatusHistory.Count);
            foreach (var statusHistory in savedDirection.DirectionStatusHistory)
            {
                Console.WriteLine($"After Saving - StatusHistory ID: {statusHistory.Id}, DateTime: {statusHistory.DateTime}, DirectionStatusId: {statusHistory.DirectionStatusId}, UserFio: {statusHistory.UserFio}, Comment: {statusHistory.Comment}");
            }
            Console.WriteLine("Saved Indicators count: " + savedDirection.Indicators.Count);
        }

        private class ResponseWrapper
        {
            [JsonPropertyName("data")]
            public IEnumerable<Direction> Data { get; set; }
        }

        private class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
        }

        public class DateTimeConverterHandlingUTC : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var dateTime = reader.GetDateTime();
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            }
        }
    }
}
