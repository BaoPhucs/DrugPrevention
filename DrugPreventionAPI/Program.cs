
using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using DrugPreventionAPI.Models;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DrugPreventionAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Đăng ký DbContext
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration
                    .GetConnectionString("DefaultConnection")));

            // 2. Đăng ký các Service/Repository
            builder.Services.AddScoped<IInquiryAssignmentRepository, InquiryAssignmentRepository>();
            builder.Services.AddScoped<IInquiryCommentRepository, InquiryCommentRepository>();
            builder.Services.AddScoped<IUserInquiryRepository, UserInquiryRepository>();
            builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICourseMaterialRepository, CourseMaterialRepository>();
            builder.Services.AddScoped<ICourseEnrollmentRepository, CourseEnrollmentRepository>();
            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
            builder.Services.AddScoped<ISurveyQuestionRepository, SurveyQuestionRepository>();
            builder.Services.AddScoped<ISurveySubmissionRepository, SurveySubmissionRepository>();
            builder.Services.AddScoped<IConsultationNoteRepository, ConsultationNoteRepository>();
            builder.Services.AddScoped<IAppointmentRequestRepository, AppointmentRequestRepository>();
            builder.Services.AddScoped<IConsultantScheduleRepository, ConsultantScheduleRepository>();
            builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
            builder.Services.AddScoped<ISurveySubstanceRepository, SurveySubstanceRepository>();
            // Email service
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailService, EmailService>();

            // AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // 3. Cấu hình JWT Authentication
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"]);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // 4. Authorization
            builder.Services.AddAuthorization();

            // 5. Controllers
            builder.Services.AddControllers();

            // 6. Swagger/OpenAPI với JWT support
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập vào \"Bearer {token}\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // 1) Đăng ký CORS và cho phép header Authorization
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                      .WithOrigins("http://localhost:5173/")   // hoặc .AllowAnyOrigin() khi dev
                      .AllowAnyMethod()
                      .WithHeaders("Content-Type", "Authorization"); // <-- thêm Authorization ở đây
                });
            });

            var app = builder.Build();

            // 7. Middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DrugPreventionAPI v1");
                });
            }

            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
