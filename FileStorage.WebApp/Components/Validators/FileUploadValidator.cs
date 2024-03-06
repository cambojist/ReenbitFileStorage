using FileStorage.WebApp.Components.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace FileStorage.WebApp.Components.Validators;

public class FileUploadValidator : AbstractValidator<FileUploadRequest>
{
    private const string DOCX_FILE = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    public FileUploadValidator()
    {
        RuleFor(x => x.File)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Please upload a file.")
            .Must(IsValidDocxFile).WithMessage("Please upload a valid DOCX file.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }

    private bool IsValidDocxFile(IBrowserFile file)
    {
        return file.ContentType.Equals(DOCX_FILE, StringComparison.OrdinalIgnoreCase);
    }
}
