using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;
using MyApi.DTOs;

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
                    Console.WriteLine($"ID: {direction.Id}, StatusHistories: {direction.DirectionStatusHistory?.Count}, Indicators: {direction.Indicators?.Count}");
                    if (direction.DirectionStatusHistory != null)
                    {
                        foreach (var statusHistory in direction.DirectionStatusHistory)
                        {
                            Console.WriteLine($"StatusHistory ID: {statusHistory.Id}, DateTime: {statusHistory.DateTime}, DirectionStatusId: {statusHistory.DirectionStatusId}, UserFio: {statusHistory.UserFio}, Comment: {statusHistory.Comment}");
                        }
                    }
                }

                if (responseData?.Data != null)
                {
                    foreach (var direction in responseData.Data)
                    {
                        if (direction != null)
                        {
                            Console.WriteLine($"Deserialized - ID: {direction.Id}, StatusHistories: {direction.DirectionStatusHistory?.Count}, Indicators: {direction.Indicators?.Count}");
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
                    Indicators = d.Indicators?.Select(i => new IndicatorDto
                    {
                        IndicatorId = i.IndicatorId,
                        DirectionId = i.DirectionId,
                        Name = i.Name,
                        Value = i.Value,
                        Unit = i.Unit,
                        ReferenceRange = i.ReferenceRange,
                        Comment = i.Comment
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

        private async Task HandleDirection(Direction direction)
        {
            if (direction == null)
            {
                Console.WriteLine("Direction is null");
                return;
            }

            Console.WriteLine($"Processing Direction ID: {direction.Id}, Patient ID: {direction.Patient?.PatientId}, Laboratory ID: {direction.LaboratoryId}");

            // Ensure collections are initialized
            direction.DirectionStatusHistory = direction.DirectionStatusHistory ?? new List<DirectionStatusHistory>();
            direction.Indicators = direction.Indicators ?? new List<Indicator>();

            // Log initial data
            Console.WriteLine("Initial DirectionStatusHistory count: " + direction.DirectionStatusHistory.Count);
            Console.WriteLine("Initial Indicators count: " + direction.Indicators.Count);

            foreach (var statusHistory in direction.DirectionStatusHistory)
            {
                Console.WriteLine($"Before Saving - StatusHistory ID: {statusHistory.Id}, DateTime: {statusHistory.DateTime}, DirectionStatusId: {statusHistory.DirectionStatusId}, UserFio: {statusHistory.UserFio}, Comment: {statusHistory.Comment}");
            }

            // Check if Patient exists in Patients table
            var existingPatient = await _dbContext.Patients.FindAsync(direction.Patient.Id);
            if (existingPatient == null)
            {
                Console.WriteLine("Patient not found, adding new");
                _dbContext.Patients.Add(direction.Patient);
                await _dbContext.SaveChangesAsync(); // Save changes to get the PatientId populated
            }
            else
            {
                Console.WriteLine("Patient already exists, updating existing patient");
                _dbContext.Entry(existingPatient).CurrentValues.SetValues(direction.Patient);
                direction.Patient = existingPatient; // Reassign the tracked entity
            }

            // Ensure the PatientId is set on the Direction
            direction.PatientId = direction.Patient.Id;

            // Handling Laboratory
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

            // Handling AnalysType
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

            // Handling optional Department
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

            // Handling Direction
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

                // Clear and Add StatusHistories
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

                // Clear and Add Indicators
                existingDirection.Indicators.Clear();
                foreach (var indicator in direction.Indicators)
                {
                    Console.WriteLine($"Adding Indicator ID: {indicator.IndicatorId}");
                    existingDirection.Indicators.Add(new Indicator
                    {
                        IndicatorId = indicator.IndicatorId,
                        DirectionId = indicator.DirectionId,
                        Name = indicator.Name,
                        Value = indicator.Value,
                        Unit = indicator.Unit,
                        ReferenceRange = indicator.ReferenceRange,
                        Comment = indicator.Comment
                    });
                }
            }

            Console.WriteLine("Saving changes to database...");
            await _dbContext.SaveChangesAsync();

            // Log post-save data
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
