using FluentValidation;

namespace Attendances.Application.Commons.Models;

public class PagedRange
{
    private static readonly int DefaultPageSize = 10, DefaultPageIndex = 1;

    public int PageIndex { get; init; } = DefaultPageIndex;
    public int ListSize { get; init; } = DefaultPageSize;
    
    public virtual int From => (PageIndex - 1) * ListSize;
    public virtual int To => From + ListSize;
}

public class PagedRangeValidator : AbstractValidator<PagedRange>
{
    public PagedRangeValidator()
    {
        RuleFor(item => item.PageIndex)
            .NotEmpty().WithMessage("Page index cannot be empty")
            .GreaterThan(0).WithMessage("Page index must be greater than 0");
        RuleFor(item => item.ListSize)
            .NotEmpty().WithMessage("List size cannot be empty")
            .GreaterThan(0).WithMessage("Page index must be greater than 0");
    }
}