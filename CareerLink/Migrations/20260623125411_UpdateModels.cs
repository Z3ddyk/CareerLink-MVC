using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerLink.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "EmployerProfiles");

            migrationBuilder.RenameColumn(
                name: "LinkedInUrl",
                table: "JobSeekerProfiles",
                newName: "LinkedinUrl");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "JobSeekerProfiles",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "JobSeekerProfiles",
                newName: "GithubUrl");

            migrationBuilder.RenameColumn(
                name: "ExperienceYears",
                table: "JobSeekerProfiles",
                newName: "Bio");

            migrationBuilder.RenameColumn(
                name: "EducationLevel",
                table: "JobSeekerProfiles",
                newName: "Availability");

            migrationBuilder.RenameColumn(
                name: "Website",
                table: "EmployerProfiles",
                newName: "WebsiteUrl");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "EmployerProfiles",
                newName: "Headquarters");

            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "EmployerProfiles",
                newName: "CompanyLogoUrl");

            migrationBuilder.AddColumn<int>(
                name: "YearsExperience",
                table: "JobSeekerProfiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoundedYear",
                table: "EmployerProfiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "EmployerProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearsExperience",
                table: "JobSeekerProfiles");

            migrationBuilder.DropColumn(
                name: "FoundedYear",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "EmployerProfiles");

            migrationBuilder.RenameColumn(
                name: "LinkedinUrl",
                table: "JobSeekerProfiles",
                newName: "LinkedInUrl");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "JobSeekerProfiles",
                newName: "Summary");

            migrationBuilder.RenameColumn(
                name: "GithubUrl",
                table: "JobSeekerProfiles",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Bio",
                table: "JobSeekerProfiles",
                newName: "ExperienceYears");

            migrationBuilder.RenameColumn(
                name: "Availability",
                table: "JobSeekerProfiles",
                newName: "EducationLevel");

            migrationBuilder.RenameColumn(
                name: "WebsiteUrl",
                table: "EmployerProfiles",
                newName: "Website");

            migrationBuilder.RenameColumn(
                name: "Headquarters",
                table: "EmployerProfiles",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "CompanyLogoUrl",
                table: "EmployerProfiles",
                newName: "LogoUrl");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "EmployerProfiles",
                type: "TEXT",
                nullable: true);
        }
    }
}
