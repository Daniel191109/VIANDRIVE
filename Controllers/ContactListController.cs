using Microsoft.AspNetCore.Mvc;
using VIANDRIVEFULL.Services;

public class ContactListController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactListController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet("GetAllContactListData")]
    public IActionResult GetAllContactListData()
    {
        var data = _contactService.GetAllContactListData();
        return Ok(data);
    }
}
