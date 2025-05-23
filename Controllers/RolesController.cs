using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory_Managment_System.DTOs;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Managment_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(RoleManager<IdentityRole> roleManager, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles
                .Where(r => r.Id != null && r.Name != null)
                .Select(r => new RoleModel { Id = r.Id!, Name = r.Name! })
                .ToList();
            _logger.LogInformation("Loaded {RoleCount} roles for Index view", roles.Count);
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole { Name = model.Name };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Role created successfully: {RoleName}", model.Name);
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogWarning("Role creation error: {Error}", error.Description);
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state for role creation: {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Edit action called with null or empty ID");
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role not found for ID: {Id}", id);
                return NotFound();
            }
            var model = new RoleModel { Id = role.Id, Name = role.Name };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, RoleModel model)
        {
            if (string.IsNullOrEmpty(id) || id != model.Id)
            {
                _logger.LogWarning("Edit action called with invalid ID: {Id}", id);
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    _logger.LogWarning("Role not found for ID: {Id}", id);
                    return NotFound();
                }
                role.Name = model.Name;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Role updated successfully: {RoleName}", model.Name);
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogWarning("Role update error: {Error}", error.Description);
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state for role edit: {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Delete action called with null or empty ID");
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role not found for ID: {Id}", id);
                return NotFound();
            }
            var model = new RoleModel { Id = role.Id, Name = role.Name };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role not found for ID: {Id}", id);
                return NotFound();
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role deleted successfully: {RoleName}", role.Name);
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogWarning("Role deletion error: {Error}", error.Description);
            }
            return View(new RoleModel { Id = role.Id, Name = role.Name });
        }
    }
}