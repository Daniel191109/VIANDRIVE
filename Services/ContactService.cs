using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Services;

public class ContactService : IContactService
{
    private readonly AppDbContext _context;

    public ContactService(AppDbContext context)
    {
        _context = context;
    }

    public List<ContactDto> GetAllContactListData()
    {
        return _context.ContactList
            .Select(c => new ContactDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Telefono = c.Telefono
            }).ToList();
    }
}
