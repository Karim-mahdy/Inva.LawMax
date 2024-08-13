using System;
using Inva.LawMax.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Inva.LawMax.Books;

public class BookAppService :
    CrudAppService<
        Book, //The Book entity
        BookDto, //Used to show books
        Guid, //Primary key of the book entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateBookDto>, //Used to create/update a book
    IBookAppService //implement the IBookAppService
{
    public BookAppService(IRepository<Book, Guid> repository)
        : base(repository)
    {
        GetPolicyName = LawMaxPermissions.Books.Default;
        GetListPolicyName = LawMaxPermissions.Books.Default;
        CreatePolicyName = LawMaxPermissions.Books.Create;
        UpdatePolicyName = LawMaxPermissions.Books.Edit;
        DeletePolicyName = LawMaxPermissions.Books.Delete;
    }
}