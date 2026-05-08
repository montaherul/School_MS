using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Controllers;

public class RoleController : GenericCrudController<Role>
{
    private readonly SchoolDbContext _db;
    public RoleController(SchoolDbContext db) : base(db, "Role") { _db = db; }

    public override async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var items = new List<dynamic>();
        int totalCount = 0;

        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetRoleList";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));

            await _db.Database.OpenConnectionAsync(cancellationToken);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        PermissionCount = reader.GetInt32(3),
                        UserCount = reader.GetInt32(4)
                    });
                    totalCount = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                }
            }
            await _db.Database.CloseConnectionAsync();
        }

        if (Request.Headers["Accept"].ToString().Contains("application/json") || Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Query.ContainsKey("page"))
        {
            return Json(new { data = items, last_page = Math.Ceiling((double)totalCount / pageSize), total_records = totalCount });
        }

        return View();
    }

}
