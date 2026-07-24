using BloodBankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BloodBankApp.Controllers
{
    public class DonorController : Controller
    {
        private readonly BloodBankDbContext _context;
        public DonorController(BloodBankDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var donors = _context.Donors.ToList();
            return View(donors);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string FullName, string BloodGroup, string ContactNo, string City, DateTime LastDonationDate)
        {
            
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(BloodGroup) || string.IsNullOrEmpty(ContactNo))
            {
              
                ViewBag.ErrorMessage = "Full Name, Blood Group, and Contact No are required!";
                return View();
            }

            var newDonor = new Donor
            {
                FullName = FullName,
                BloodGroup = BloodGroup,
                ContactNo = ContactNo,
                City = City,
                LastDonationDate = LastDonationDate
            };
            _context.Donors.Add(newDonor);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var donor = _context.Donors.Find(id);

            if (donor == null)
            {
                return NotFound();
            }

            return View(donor);
        }

        [HttpPost]
        public IActionResult Edit(int DonorId, string FullName, string BloodGroup, string ContactNo, string City, DateTime LastDonationDate)
        {
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(BloodGroup))
            {
                ViewBag.ErrorMessage = "Full Name and Blood Group are required.";
                var donor = _context.Donors.Find(DonorId);
                return View(donor);
            }

            var existingDonor = _context.Donors.Find(DonorId);
            if (existingDonor != null)
            {
                existingDonor.FullName = FullName;
                existingDonor.BloodGroup = BloodGroup;
                existingDonor.ContactNo = ContactNo;
                existingDonor.City = City;
                existingDonor.LastDonationDate = LastDonationDate;

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var donor = _context.Donors.Find(id);

            if (donor != null)
            {
                _context.Donors.Remove(donor);

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Filter(string bloodGroup)
        {
            var donors = _context.Donors
                                 .Where(d => d.BloodGroup == bloodGroup)
                                 .ToList();

            ViewBag.SelectedGroup = bloodGroup;
            return View(donors);
        }
        public IActionResult Recent()
        {
            var recentDonors = _context.Donors
                                       .OrderByDescending(d => d.LastDonationDate)
                                       .ToList();
            return View(recentDonors);
        }
        public IActionResult Stats()
        {
            var donorStats = _context.Donors
                                     .Select(d => new {
                                         d.FullName,
                                         Count = _context.Donations.Count(don => don.DonorId == d.DonorId)
                                     }).ToList();

            ViewBag.Stats = donorStats;
            return View();
        }
        public IActionResult TotalVolume()
        {
            var totalVolume = _context.Donations.Sum(d => d.VolumeMl);

            ViewBag.TotalVolume = totalVolume;
            return View();
        }

    }
}
