using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Garage25.Models;

namespace Garage25.Data
{
    public static class SeedData
    {
        internal static readonly string[] vtypes =
        {
            "Airplane", "Bicycle", "Boat", "Bus", "Car", "Lorry", "Moped", "Motorcycle", "Train", "Truck"
        };

        internal static void Initialize(IServiceProvider services)
        {
            var options = services.GetRequiredService<DbContextOptions<Garage25Context>>();
            using (var context = new Garage25Context(options))
            {
                if (context.Member.Any())
                {
                    context.Member.RemoveRange(context.Member);
                    context.ParkedVehicle.RemoveRange(context.ParkedVehicle);
                    context.VehicleType.RemoveRange(context.VehicleType);
                }
                var members = new List<Member>();
                for (int i = 0; i < 10; i++)
                {
                    var userName = Faker.Name.First();
                    var member = new Member
                    {
                        UserName = userName,
                        Email = Faker.Internet.Email(userName)
                    };
                    members.Add(member);
                }
                context.Member.AddRange(members);
                Random random = new Random();
                var vehicletypes = new List<VehicleType>();
                for (int i = 0; i < 10; i++)
                {
                    var vehicletype = new VehicleType
                    {
                        Name = vtypes[i]
                    };

                    vehicletypes.Add(vehicletype);
                }
                context.VehicleType.AddRange(vehicletypes);
                context.SaveChanges();
                var memberIds = new List<int>();
                foreach (var member in members)
                {
                    memberIds.Add(member.Id);
                }
                var vehicleTypeIds = new List<int>();
                foreach (var vehicletype in vehicletypes)
                {
                    vehicleTypeIds.Add(vehicletype.Id);
                }
                var parkedVehicles = new List<ParkedVehicle>();
                foreach (var member in members)
                {
                    foreach (var vehicletype in vehicletypes)
                    {
                        var parkedVehicle = new ParkedVehicle
                        {
                            RegNum = GetBogusData(BogusEnum.RegNum),
                            Color = GetBogusData(BogusEnum.Color),
                            CheckInTime = GetBogusData(),
                            MemberId = memberIds[random.Next(0, 9)],
                            VehicleTypeId = vehicleTypeIds[random.Next(0, 9)],
                        };
                        parkedVehicles.Add(parkedVehicle);
                    }
                }
                context.ParkedVehicle.AddRange(parkedVehicles);
                context.SaveChanges();
            }
        }

        public enum BogusEnum
        {
            Color, RegNum
        }
        internal static string GetBogusData(BogusEnum type)
        {
            if (type == BogusEnum.Color)
            {
                var color = new Bogus.DataSets.Commerce(locale: "en");
                var textInfo = new CultureInfo("en", false).TextInfo;
                return textInfo.ToTitleCase(color.Color());
            }
            else if (type == BogusEnum.RegNum)
            {
                return new Bogus.DataSets.Vehicle().Vin().Substring(0, 6);
            }
            return "0";
        }

        internal static System.DateTime GetBogusData()
        {
            Random random = new Random();
            return DateTime.Now.
                AddDays(random.Next(-10, 0))
                .AddHours(random.Next(0, 24))
                .AddMinutes(random.Next(0, 60))
                .AddSeconds(random.Next(0, 60));
        }
    }
}
