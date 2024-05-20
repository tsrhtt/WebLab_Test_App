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

            try
            {
                var direction = JsonSerializer.Deserialize<Direction>(content, new JsonSerializerOptions
                {
                    Converters = { new DateTimeConverterHandlingUTC() }
                });

                // Debugging output
                Console.WriteLine($"Deserialized - ID: {direction.Id}, StatusHistories: {direction.DirectionStatusHistory?.Count}, IndicatorGroups: {direction.IndicatorGroups?.Count}");
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

                await HandleDirection(direction);

                return new DirectionDto
                {
                    Id = direction.Id,
                    PatientId = direction.PatientId,
                    Patient = new PatientDto
                    {
                        Id = direction.Patient.Id,
                        IdentificationNumber = direction.Patient.IdentificationNumber,
                        PatientId = direction.Patient.PatientId,
                        Name = direction.Patient.Name,
                        Surname = direction.Patient.Surname,
                        SecondName = direction.Patient.SecondName,
                        FullName = direction.Patient.FullName,
                        Sex = direction.Patient.Sex,
                        BirthDate = direction.Patient.BirthDate,
                        SexDescription = direction.Patient.SexDescription,
                        Age = direction.Patient.Age
                    },
                    LaboratoryId = direction.LaboratoryId,
                    Laboratory = direction.Laboratory,
                    AnalysTypeId = direction.AnalysTypeId,
                    AnalysTypeName = direction.AnalysTypeName,
                    AnalysTypeFormat = direction.AnalysTypeFormat,
                    DepartmentId = direction.DepartmentId,
                    DepartmentName = direction.DepartmentName,
                    DirectionStatusHistory = direction.DirectionStatusHistory?.Select(sh => new DirectionStatusHistoryDto
                    {
                        Id = sh.Id,
                        DirectionId = sh.DirectionId,
                        DateTime = sh.DateTime,
                        DirectionStatusId = sh.DirectionStatusId,
                        UserFio = sh.UserFio,
                        Comment = sh.Comment
                    }).ToList(),
                    Indicators = direction.IndicatorGroups?.SelectMany(ig => ig.Value).Select(i => new IndicatorDto
                    {
                        Id = i.Id,
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
                    PatientFullName = direction.PatientFullName,
                    Cito = direction.Cito,
                    IsArchived = direction.IsArchived,
                    DirectionStatus = direction.DirectionStatus,
                    DirectionStatusId = direction.DirectionStatusId,
                    Category = direction.Category,
                    RequestDate = direction.RequestDate,
                    RequestedBy = direction.RequestedBy,
                    AcceptedDate = direction.AcceptedDate,
                    AcceptedBy = direction.AcceptedBy,
                    OnDate = direction.OnDate,
                    ReadyDate = direction.ReadyDate,
                    Sid = direction.Sid,
                    HasAnyResults = direction.HasAnyResults,
                    LaborantComment = direction.LaborantComment,
                    SamplingDate = direction.SamplingDate,
                    SamplingDateStr = direction.SamplingDateStr,
                    SampleNumber = direction.SampleNumber,
                    SamplingDoctorFio = direction.SamplingDoctorFio,
                    DoctorLabDiagnosticFio = direction.DoctorLabDiagnosticFio,
                    DoctorFeldsherLaborantFio = direction.DoctorFeldsherLaborantFio,
                    DoctorBiologFio = direction.DoctorBiologFio,
                    BioMaterialCount = direction.BioMaterialCount,
                    BioMaterialType = direction.BioMaterialType,
                    NumberByJournal = direction.NumberByJournal
                };
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JsonException encountered: {ex.Message}");
                throw;
            }
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

                existingDirection.Indicators.Clear();
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
