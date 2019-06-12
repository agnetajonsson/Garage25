using Garage25.Data;
using Garage25.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Garage25.Controllers
{
    public enum PVSearchTerm
    {
        RegNum,
        VehicleType
    }

    public enum PV2SearchTerm
    {
        RegNum,
        Color
    }
    public class ParkedVehiclesController : Controller
    {
        private readonly Garage25Context _context;

        public ParkedVehiclesController(Garage25Context context)
        {
            _context = context;
        }

        // GET: ParkedVehicles
        //public async Task<IActionResult> Index2()
        //{
        //    var result = CreateParkedVehicleViewModels();

        //    return View(await result.OrderBy(p => p.RegNum).ToListAsync());
        //}

        public async Task<IActionResult> Index2(string sortOrder)
        {
            var result = CreateParkedVehicleViewModels();

            ViewData["RegNumSortOrder"] = string.IsNullOrEmpty(sortOrder) ? "RegNum_desc" : "";
            ViewData["OwnerSortOrder"] = sortOrder == "Owner" ? "Owner_desc" : "Owner";
            ViewData["VehicleTypeSortOrder"] = sortOrder == "VehicleType" ? "VehicleType_desc" : "VehicleType";
            ViewData["ParkingTimeSortOrder"] = sortOrder == "ParkingTime" ? "ParkingTime_desc" : "ParkingTime";

            switch (sortOrder)
            {
                case "RegNum_desc":
                    result = result.OrderByDescending(p => p.RegNum);
                    TempData["message"] = "Sorting \'Registration Number\' descending";
                    break;
                case "Owner":
                    result = result.OrderBy(p => p.Owner);
                    TempData["message"] = "Sorting \'Owner\' ascending";
                    break;
                case "Owner_desc":
                    result = result.OrderByDescending(p => p.Owner);
                    TempData["message"] = "Sorting \'Owner\' descending";
                    break;
                case "VehicleType":
                    result = result.OrderBy(p => p.VehicleType);
                    TempData["message"] = "Sorting \'VehicleType\' ascending";
                    break;
                case "VehicleType_desc":
                    result = result.OrderByDescending(p => p.VehicleType);
                    TempData["message"] = "Sorting \'VehicleType\' descending";
                    break;
                case "ParkingTime":
                    result = result.OrderBy(p => p.ParkingTime);
                    TempData["message"] = "Sorting \'ParkingTime\' ascending";
                    break;
                case "ParkingTime_desc":
                    result = result.OrderByDescending(p => p.ParkingTime);
                    TempData["message"] = "Sorting \'ParkingTime\' descending";
                    break;
                default:
                    result = result.OrderBy(m => m.RegNum);
                    if (TempData["message"] == null)
                        TempData["message"] = "Sorting \'Registration Number\' ascending";
                    else if (!TempData["message"].ToString().Contains("Vehicle Type"))
                        TempData["message"] = "Sorting \'Registration Number\' ascending";
                    break;
            }

            return View(nameof(Index2), await result.ToListAsync());
        }

            // GET: ParkedVehicles
            //public async Task<IActionResult> Index()
            //{
            //    return View(await _context.ParkedVehicle
            //                            .OrderBy(p => p.RegNum)
            //                            .ToListAsync());
            //}

            public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["RegNumSortOrder"] = string.IsNullOrEmpty(sortOrder) ? "RegNum_desc" : "";
            ViewData["ColorSortOrder"] = sortOrder == "Color" ? "Color_desc" : "Color";
            ViewData["CheckInTimeSortOrder"] = sortOrder == "CheckInTime" ? "CheckInTime_desc" : "CheckInTime";

            IQueryable<ParkedVehicle> result = _context.ParkedVehicle;

            switch (sortOrder)
            {
                case "RegNum_desc":
                    result = result.OrderByDescending(m => m.RegNum);
                    TempData["message"] = "Sorting \'Registration Number\' descending";
                    break;
                case "Color":
                    result = result.OrderBy(m => m.Color);
                    TempData["message"] = "Sorting \'Color\' ascending";
                    break;
                case "Color_desc":
                    result = result.OrderByDescending(m => m.Color);
                    TempData["message"] = "Sorting \'Color\' descending";
                    break;
                case "CheckInTime":
                    result = result.OrderBy(m => m.CheckInTime);
                    TempData["message"] = "Sorting \'Check In Time\' ascending";
                    break;
                case "CheckInTime_desc":
                    result = result.OrderByDescending(m => m.CheckInTime);
                    TempData["message"] = "Sorting \'Check In Time\' descending";
                    break;
                default:
                    result = result.OrderBy(m => m.RegNum);
                    if (TempData["message"] == null)
                        TempData["message"] = "Sorting \'Registration Number\' ascending";
                    else if (!TempData["message"].ToString().Contains("Vehicle"))
                        TempData["message"] = "Sorting \'Registration Number\' ascending";
                    break;
            }

            return View(nameof(Index), await result.ToListAsync());
        }

        // GET: ParkedVehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkedVehicle = await _context.ParkedVehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkedVehicle == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Id == parkedVehicle.MemberId);
            if (member == null)
            {
                return NotFound();
            }
            ViewData["membername"] = member.UserName;

            var vehicletype = await _context.VehicleType
                .FirstOrDefaultAsync(m => m.Id == parkedVehicle.VehicleTypeId);
            if (vehicletype == null)
            {
                return NotFound();
            }
            ViewData["vehicletypename"] = vehicletype.Name;

            return View(parkedVehicle);
        }

        // GET: ParkedVehicles/Create
        public IActionResult Create2()
        {
            var createPVViewModel = new CreatePVViewModel
            {
                Id = 0,
                RegNum = SeedData.GetBogusData(SeedData.BogusEnum.RegNum),
                Color = SeedData.GetBogusData(SeedData.BogusEnum.Color),
                UserName = ""
            };

            ViewData["UserName"] = new SelectList(_context.Member
                                                   .OrderBy(m => m.UserName), "UserName", "UserName");

            ViewData["TypeName"] = new SelectList(_context.VehicleType
                                                    .OrderBy(v => v.Name), "Name", "Name");

            return View(createPVViewModel);
        }

        // POST: ParkedVehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create2([Bind("RegNum,Color,UserName,TypeName")] CreatePVViewModel createPVViewModel)
        {
            if (await _context.ParkedVehicle.AnyAsync(p => p.RegNum == createPVViewModel.RegNum))
            {
                ModelState.AddModelError("RegNum", $"\'{createPVViewModel.RegNum}\' already exists!");
            }

            if (!await _context.Member.AnyAsync(m => m.UserName == createPVViewModel.UserName))
            {
                ModelState.AddModelError("UserName", $"\'{createPVViewModel.UserName}\' do not exists!");
            }

            if (!await _context.VehicleType.AnyAsync(v => v.Name == createPVViewModel.TypeName))
            {
                ModelState.AddModelError("TypeName", $"\'{createPVViewModel.TypeName}\' do not exists!");
            }

            if (ModelState.IsValid)
            {
                Member member = await _context.Member
                                .FirstOrDefaultAsync(m => m.UserName == createPVViewModel.UserName);
                if (member == null)
                {
                    return NotFound();
                }

                VehicleType vehicleType = await _context.VehicleType
                                .FirstOrDefaultAsync(v => v.Name == createPVViewModel.TypeName);
                if (vehicleType == null)
                {
                    return NotFound();
                }

                DateTime now = DateTime.Now;
                ParkedVehicle parkedVehicle = new ParkedVehicle
                {
                    RegNum = createPVViewModel.RegNum,
                    Color = createPVViewModel.Color,
                    CheckInTime = now,
                    MemberId = member.Id,
                    Member = member,
                    VehicleTypeId = vehicleType.Id,
                    VehicleType = vehicleType
                };

                _context.Add(parkedVehicle);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Vehicle \'{parkedVehicle.RegNum}\' is checked in";

                return RedirectToAction(nameof(Index));
            }

            ViewData["UserName"] = new SelectList(_context.Member
                                                   .OrderBy(m => m.UserName), "UserName", "UserName");

            ViewData["TypeName"] = new SelectList(_context.VehicleType
                                                   .OrderBy(v => v.Name), "Name", "Name");

            return View(createPVViewModel);
        }

        // GET: ParkedVehicles/Create
        public IActionResult Create()
        {
            // Add some bogus data
            DateTime now = DateTime.Now;
            var vehicle = new Bogus.DataSets.Vehicle();
            var parkedVehicle = new ParkedVehicle
            {
                RegNum = vehicle.Vin().Substring(0, 6),
                CheckInTime = now
            };
            return View(parkedVehicle);
        }

        // POST: ParkedVehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RegNum,Color,Member.UserName")] ParkedVehicle parkedVehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parkedVehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index2));
            }
            return View(parkedVehicle);
        }

        public async Task<IActionResult> Filter(string search, string reset, string searchterm)
        {
           // IQueryable<Member> result = _context.ParkedVehicles;
            var result = CreateParkedVehicleViewModels();

            if (string.IsNullOrWhiteSpace(reset) && !string.IsNullOrWhiteSpace(searchterm))
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    ViewData["Search"] = search;
                    ViewData["Select"] = searchterm;

                    //  search = search.ToUpper();

                    switch ((PVSearchTerm)int.Parse(searchterm))
                    {
                        case PVSearchTerm.RegNum:
                            result = result.Where(m => m.RegNum.Contains(search, StringComparison.CurrentCultureIgnoreCase));
                            break;
                        case PVSearchTerm.VehicleType:
                            result = result.Where(m => m.VehicleType.Contains(search, StringComparison.CurrentCultureIgnoreCase));
                            break;
                        default:
                            break;
                    }
                }
            }

          //  await UpdateParkedVehicles();

            return View(nameof(Index2), await result
                                            .OrderBy(m => m.RegNum)
                                            .ToListAsync());
        }

        public async Task<IActionResult> Filter2(string search, string reset, string searchterm)
        {
            IQueryable<ParkedVehicle> result = _context.ParkedVehicle;
           
            if (string.IsNullOrWhiteSpace(reset) && !string.IsNullOrWhiteSpace(searchterm))
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    ViewData["Search"] = search;
                    ViewData["Select"] = searchterm;

                    //  search = search.ToUpper();

                    switch ((PV2SearchTerm)int.Parse(searchterm))
                    {
                        case PV2SearchTerm.RegNum:
                            result = result.Where(m => m.RegNum.Contains(search, StringComparison.CurrentCultureIgnoreCase));
                            break;
                        case PV2SearchTerm.Color:
                            result = result.Where(m => m.Color.Contains(search, StringComparison.CurrentCultureIgnoreCase));
                            break;
                        default:
                            break;
                    }
                }
            }

            //  await UpdateParkedVehicles();

            return View(nameof(Index), await result
                                            .OrderBy(m => m.RegNum)
                                            .ToListAsync());
        }



        // GET: ParkedVehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkedVehicle = await _context.ParkedVehicle.FindAsync(id);
            if (parkedVehicle == null)
            {
                return NotFound();
            }
            return View(parkedVehicle);
        }

        // POST: ParkedVehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RegNum,Color,CheckInTime,MemberId,VehicleTypeId")] ParkedVehicle parkedVehicle)
        {
            if (id != parkedVehicle.Id)
            {
                return NotFound();
            }

            // Check RegNum
            if ( _context.ParkedVehicle.AsNoTracking().Any(p => p.Id == parkedVehicle.Id) &&
                 _context.ParkedVehicle.AsNoTracking().First(p => p.Id == parkedVehicle.Id).RegNum != parkedVehicle.RegNum &&
                 _context.ParkedVehicle.AsNoTracking().Any(m => m.RegNum == parkedVehicle.RegNum))
            {
                ModelState.AddModelError("RegNum", $"\'{parkedVehicle.RegNum}\' already exists!");
            }

            //if (await _context.ParkedVehicle.AnyAsync(p => p.RegNum == parkedVehicle.RegNum))
            //{
            //    ModelState.AddModelError("RegNum", $"\'{parkedVehicle.RegNum}\' already exists");
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parkedVehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkedVehicleExists(parkedVehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Message"] = $"Vehicle \'{parkedVehicle.RegNum}\' is updated";

                return RedirectToAction(nameof(Index));
            }
            return View(parkedVehicle);
        }

        // GET: ParkedVehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkedVehicle = await _context.ParkedVehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkedVehicle == null)
            {
                return NotFound();
            }

            return View(parkedVehicle);
        }

        //// POST: ParkedVehicles/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var parkedVehicle = await _context.ParkedVehicle.FindAsync(id);
        //    _context.ParkedVehicle.Remove(parkedVehicle);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        // POST: ParkedVehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOutConfirmed(int id)
        {
            DateTime now = DateTime.Now;
            var parkedVehicle = await _context.ParkedVehicle.FindAsync(id);
            var model = new RecieptViewModel
            {
                RegNum = parkedVehicle.RegNum,
                Color = parkedVehicle.Color,
                CheckInTime = parkedVehicle.CheckInTime,
                CheckOutTime = now,
                TotalTime = now - parkedVehicle.CheckInTime,
                VehicleType = _context.VehicleType.Any(v => v.Id == parkedVehicle.VehicleTypeId) ?
                                    _context.VehicleType.First(v => v.Id == parkedVehicle.VehicleTypeId).Name : ""
            };
            _context.ParkedVehicle.Remove(parkedVehicle);
            await _context.SaveChangesAsync();

            //var Parkedhours = model.CheckOutTime - model.CheckInTime;
            //model.TotalTime = string.Format("{0:D2}:{1:D2}:{2:D2}", Parkedhours.Days, Parkedhours.Hours, Parkedhours.Minutes);
            //model.Price = String.Format("{0:F2}", Parkedhours.TotalHours * 10); ;
            // TempData["Message"] = $"Vehicle \'{parkedVehicle.RegNum}\' is checked out";
            //var model = new RecieptViewModel
            //{
            //    RegNum = parkedVehicle.RegNum,
            //    Color = parkedVehicle.Color,
            //    CheckInTime = parkedVehicle.CheckInTime,
            //    CheckOutTime = DateTime.Now,
            //    TotalTime = DateTime.Now - parkedVehicle.CheckInTime,
            //    VehicleType = _context.VehicleType.Any(v => v.Id == parkedVehicle.VehicleTypeId) ?
            //                        _context.VehicleType.First(v => v.Id == parkedVehicle.VehicleTypeId).Name : ""
            //};

            return View("Reciept", model);
        }

        private bool ParkedVehicleExists(int id)
        {
            return _context.ParkedVehicle.Any(e => e.Id == id);
        }

        private void CreateSelectList()
        {
            //List<VehicleType> vehicleTypes = new List<VehicleType> {
            //    new VehicleType { Name = "Airplane" },
            //    new VehicleType { Name = "Bicycle" },
            //    new VehicleType { Name = "Boat" },
            //    new VehicleType { Name = "Bus" },
            //    new VehicleType { Name = "Car" },
            //    new VehicleType { Name = "Lorry" },
            //    new VehicleType { Name = "Moped" },
            //    new VehicleType { Name = "Motorcycle" },
            //    new VehicleType { Name = "Train" },
            //    new VehicleType { Name = "Truck" }
            //};

            ViewData["Name"] = new SelectList(_context.VehicleType, "Name", "Name");
        }

        private IQueryable<ParkedVehicleViewModel> CreateParkedVehicleViewModels()
        {
            return _context.ParkedVehicle
                     .Select( p => new ParkedVehicleViewModel
                     {
                         Id = p.Id,
                         RegNum = p.RegNum,
                         ParkingTime = DateTime.Now - p.CheckInTime,
                         Owner = _context.Member.Any(m => m.Id == p.MemberId) ? 
                                     _context.Member.First(m => m.Id == p.MemberId).UserName : "",
                         VehicleType = _context.VehicleType.Any(v => v.Id == p.VehicleTypeId) ?
                                            _context.VehicleType.First(v => v.Id == p.VehicleTypeId).Name : ""
                     }); 
        }
    }
}
