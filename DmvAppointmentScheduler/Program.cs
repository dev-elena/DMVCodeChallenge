using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace DmvAppointmentScheduler
{
    class Program
    {
        public static Random random = new Random();
        public static List<Appointment> appointmentList = new List<Appointment>();
        public static ObservableCollection<Teller> occupiedTellers = new ObservableCollection<Teller>();
        static void Main(string[] args)
        {
            CustomerList customers = ReadCustomerData();
            TellerList tellers = ReadTellerData();
            Calculation(customers, tellers);
            OutputTotalLengthToConsole();

        }
        private static CustomerList ReadCustomerData()
        {
            string fileName = "CustomerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            CustomerList customerData = JsonConvert.DeserializeObject<CustomerList>(jsonString);
            return customerData;

        }
        private static TellerList ReadTellerData()
        {
            string fileName = "TellerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            TellerList tellerData = JsonConvert.DeserializeObject<TellerList>(jsonString);
            return tellerData;

        }
        static void Calculation(CustomerList customers, TellerList tellers)
        {
            // Your code goes here .....
            // Re-write this method to be more efficient instead of a assigning all customers to the same teller

            foreach (Customer customer in customers.Customer)
            {
                if (occupiedTellers.Count == tellers.Teller.Count) occupiedTellers.Clear();
                //assign Tellers with Speciality 0 first to keep Tellers with multipliers available
                var teller = tellers.Teller.Find(x => !occupiedTellers.Contains(x) && (x.specialtyType.Equals(customer.type) || Int32.Parse(x.specialtyType) == 0));
                if (teller == null)
                {
                    teller = tellers.Teller.Find(x => !occupiedTellers.Contains(x));

                }
                CreateAppoinmnent(new Appointment(customer, teller));
            }
        }
        static void CreateAppoinmnent(Appointment appointment)
        {
            appointmentList.Add(appointment);
            occupiedTellers.Add(appointment.teller);
        }
        static void OutputTotalLengthToConsole()
        {
            var tellerAppointments =
                from appointment in appointmentList
                group appointment by appointment.teller into tellerGroup
                select new
                {
                    teller = tellerGroup.Key,
                    totalTellerDuration = tellerGroup.Sum(x => x.duration),
                    longestDuration = tellerGroup.Max(appointment => appointment.duration),
                    countCustomers = tellerGroup.Select(y => y.customer).Distinct().Count()
                };

            foreach (var tellerAppointment in tellerAppointments)
            {
                Console.WriteLine($"Teller {tellerAppointment.teller.id} (speciality {tellerAppointment.teller.specialtyType}) will work for {tellerAppointment.totalTellerDuration} minutes with {tellerAppointment.countCustomers} Customers!");
            }

            var maxDuration = tellerAppointments.OrderBy(i => i.totalTellerDuration).LastOrDefault();
            var longestDuration = tellerAppointments.OrderBy(i => i.longestDuration).LastOrDefault();
            var maxCustomers = tellerAppointments.OrderBy(i => i.countCustomers).LastOrDefault();


            double totalDuration = appointmentList.Sum(appointment => appointment.duration);
            int totalCustomers = appointmentList.Select(appointment => appointment.customer).Distinct().Count();

            Console.WriteLine($"TotalDuration {totalDuration}, TotalCustomers {totalCustomers}");
            Console.WriteLine($"Teller {maxDuration.teller.id} with max duration  {maxDuration.totalTellerDuration} mins");
            Console.WriteLine($"Teller {longestDuration.teller.id} with longest appoinment  {longestDuration.longestDuration} mins");
            Console.WriteLine($"Teller {maxCustomers.teller.id} with biggest number of Customers - {maxCustomers.countCustomers} ");


        }

    }
}
