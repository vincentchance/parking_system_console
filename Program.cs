using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;  // Add this for List<T>
using System.Linq;  // Add this for LINQ operations like Contains

namespace ParkingSystem
{
    class ParkingLot
    {
		private string[][] lot; //array for parking lot
		private int availableSpots; //variable to track availableSpots
		private decimal hourlyRate = 5000m;
		
		public enum VehicleTypes
		{
			SmallCar,
			SmallMotorcycle
		}
		
		public ParkingLot(int size)
		{
			lot = new string[size][]; //initialize jagged array
			availableSpots = size;
		}
		
		public void ParkCar(string car, string vehicleType, string policeNumber, string color )
		{
			if(Enum.TryParse( vehicleType, out VehicleTypes vehicleTypeEnum))
			{
				if( vehicleTypeEnum != VehicleTypes.SmallCar && vehicleTypeEnum != VehicleTypes.SmallMotorcycle) //check out if type of vehicle is not allowed
				{
					Console.WriteLine("Only small car and small motorcycle are allowed");
					return;
				}
					
				for(int i = 0; i < lot.Length; i++)
				{
					if(lot[i] == null)
					{
							lot[i] = new string [] { (i + 1).ToString() ,policeNumber, color, car, DateTime.Now.ToString(), vehicleType };
							availableSpots--;
							Console.WriteLine($"$ park {policeNumber} {color} {car}");
							Console.WriteLine($"Allocated slot number: {i + 1}");
							return ;
					}
				}
				Console.WriteLine($"$ park {policeNumber} {color} {car}");
				Console.WriteLine("Sorry, parking lot is full");
			}
			else
			{
				Console.WriteLine($"Invalid vehicle type: {vehicleType}. Please provide a valid vehicle type");
				return;
			}
		}
		
		public void CarLeave(int lotNumber)
		{
			if( lotNumber < 1 || lotNumber > lot.Length)
			{
				Console.WriteLine("Invalid Parking slot number");
				return;
			}
			int index = lotNumber - 1; //adjust integer array based-0
			
			if(lot[index] != null)
			{
				if (lot[index].Length >= 5 && DateTime.TryParse(lot[index][4], out DateTime carParkedTime))
				{
					TimeSpan duration = DateTime.Now - carParkedTime; //calculate the duration
					double totalHours = duration.TotalHours < 1 ? 1 : duration.TotalHours;
					decimal cost = (decimal)totalHours * hourlyRate; //Apply hourlyRate
					
					Console.WriteLine($"Car {lot[index][3]} (Police Number: {lot[index][1]}, Color: {lot[index][2]}) has left the parking lot.");
					Console.WriteLine($"Parking duration: {totalHours} hour(s).");
					CultureInfo indonesiaCulture = new CultureInfo("id-ID");
					Console.WriteLine($"Total cost: {cost.ToString("C", indonesiaCulture)}");
					lot[index] = null;
					Console.WriteLine();
					if(lot[index] == null)
					{
						Console.WriteLine($"slot number {lotNumber} is free");
						return;
					}
					
				}
				else
				{
					Console.WriteLine($"Error: Invalid data in slot number {lotNumber}.");
				}
			}
			else
			{
				Console.WriteLine($"The slot number {lotNumber} is already empty");
			}
		}
		
		public void CheckParkingSlot()
		{
			Console.WriteLine("Slot\tNo.\t\tType\tColour");
			for(int i = 0; i < lot.Length; i++)
			{
				if (lot[i] == null)
				{
					continue;
				}
				else
				{
					string slotNumber = lot[i][0];
					string policeNumber = lot[i][1];
					string type = lot[i][3]; // Vehicle type (e.g., Mobil, Motor)
					string color = lot[i][2]; // Color
					Console.WriteLine($"{slotNumber}\t{policeNumber}\t{type}\t{color}");
				}
			}
			/* result must look like this in console:
				Slot    No.             Type    Colour
				1       B-1234-XYZ      Mobil   Putih
				2       B-9999-XYZ      Motor   Putih
				3       D-0001-HIJ      Mobil   Hitam
				5       B-2701-XXX      Motor   Biru
				6       B-3141-ZZZ      Mobil   Putih
			*/
		}
		
		public void ReportCarTypes()
		{
			int smallCarCount = 0;
				
			foreach ( var slot in lot )
			{
				if(slot != null)
				{
					if(slot[5].Equals("SmallCar", StringComparison.OrdinalIgnoreCase))
					{
						smallCarCount++;
					}
				}
			}
			Console.WriteLine("$ type_of vehicles Mobil");
			Console.WriteLine($"{smallCarCount}");
		}
		
		public void ReportMotorcycleTypes()
		{
			int smallMotorCount = 0;
				
			foreach ( var slot in lot )
			{
				if(slot != null)
				{
					if(slot[5].Equals("SmallMotorcycle", StringComparison.OrdinalIgnoreCase))
					{
						smallMotorCount++;
					}
				}
			}
			Console.WriteLine("$ type_of vehicles Motor");
			Console.WriteLine($"{smallMotorCount}");
		}
		
		public void CheckColor(string color)
		{
			List<String> matchingVehicles = new List<string>();
			
			foreach ( var slot in lot )
			{
				if( slot != null)
				{
					string vehicleType = slot[5]; //Type of Vehicle smallCar or smallMotorcycle
					string vehicleColor = slot[2]; //Color of vehicle
					
					if((vehicleType.Equals("SmallCar", StringComparison.OrdinalIgnoreCase) || 
					vehicleType.Equals("SmallMotorcycle", StringComparison.OrdinalIgnoreCase)) &&
					vehicleColor.Equals(color, StringComparison.OrdinalIgnoreCase))
					{
						matchingVehicles.Add(slot[1]); // Add the registration number to the list
					}
				}
				
			}
			Console.WriteLine($"$ Registration_numbers_for_vehicles_with_colour_{color.ToLower()}");
			if (matchingVehicles.Count > 0)	
			{
				Console.WriteLine(string.Join(",", matchingVehicles));
			}
			else
			{
				Console.WriteLine("No vehicles found with the specified color.");
			}
			/* result must look like this in console:
				$ slot_numbers_for_vehicles_with_colour_putih
				1 2 6
			*/
		}
		
		public void MatchPlateWithinSlot(string policePlate)
		{
			// List to store the slot numbers where the police plate matches
			List<int> matchingSlots = new List<int>();

			// Iterate through the parking lot
			for (int i = 0; i < lot.Length; i++)
			{
				// Check if the slot is occupied
				if (lot[i] != null)
				{
					// Check if the police number in the slot matches the given police plate
					if (lot[i][1].Equals(policePlate, StringComparison.OrdinalIgnoreCase))
					{
						matchingSlots.Add(i + 1);  // Add the slot number to the list (index + 1 for 1-based index)
					}
				}
			}

			// Output the result
			if (matchingSlots.Count > 0)
			{
				Console.WriteLine($"$ slot_numbers_for_registration_number_{policePlate}");
				Console.WriteLine(string.Join(" ", matchingSlots));  // Print slot numbers separated by a space
			}
			else
			{
				Console.WriteLine($"No vehicles found with the registration number: {policePlate}");
			}
			/* result must look like this in console:
				$ slot_numbers_for_registration_number B-3141-ZZZ
				6
			*/
		}
		
		public void CheckPlate()
		{
			// Create lists to store OOD plates and Event plates
			List<string> oodPlates = new List<string>();
			List<string> eventPlates = new List<string>();

			foreach (var slot in lot)
			{
				if (slot != null)
				{
					// Use regex to match the pattern
					var match = Regex.Match(slot[1], @"[A-Z]-(\d{4})-[A-Z]+");
					if (match.Success)
					{
						string number = match.Groups[1].Value;

						// Check if it's an event plate using the IsEventPlate method
						if (IsEventPlate(number))
						{
							eventPlates.Add(slot[1]);
						}
						// Check if it's an OOD plate (odd number plates)
						else if (int.TryParse(number, out int parsedNumber) && parsedNumber % 2 != 0)
						{
							oodPlates.Add(slot[1]);
						}
					}
				}
			}

			// Output OOD plates
			Console.WriteLine("$ Registration_numbers_for_vehicles_with_ood_plate");
			Console.WriteLine(string.Join(",", oodPlates));
			Console.WriteLine();
			// Output Event plates
			Console.WriteLine("$ Registration_numbers_for_vehicles_with_event_plate");
			Console.WriteLine(string.Join(",", eventPlates));
		}

		// Function to check if the plate number is an event plate
		private bool IsEventPlate(string number)
		{
			// List of known event plate patterns
			string[] eventPlateNumbers = new string[] { "1234", "3141" };

			return eventPlateNumbers.Contains(number);
		}
    }
	
	class Program
	{
		static void Main(string[] args)
        {
            Console.Write("$ create_parking_lot: ");
            int totalLot = int.Parse(Console.ReadLine());
            Console.WriteLine($"Created {totalLot} parking lot");
            Console.WriteLine();
            ParkingLot parkingLot = new ParkingLot(totalLot);

            while (true)
            {
                Console.Write("$ Enter command: ");
                string command = Console.ReadLine();

                if (command.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(" $exit");
                    break;
                }

                // Handle other commands
                switch (command)
                {
                    case "park":
                        Console.Write("Enter vehicle details (car, type, plate, color): ");
                        string[] parkDetails = Console.ReadLine().Split(',');
                        parkingLot.ParkCar(parkDetails[0], parkDetails[1], parkDetails[2], parkDetails[3]);
                        break;
                    case "leave":
                        Console.Write("$ Enter the slot number of car that want to leave: ");
                        int slotNumber = int.Parse(Console.ReadLine());
                        parkingLot.CarLeave(slotNumber);
                        break;
                    case "check":
                        parkingLot.CheckParkingSlot();
                        break;
                    case "report_car_types":
                        parkingLot.ReportCarTypes();
                        break;
                    case "report_motorcycle_types":
                        parkingLot.ReportMotorcycleTypes();
                        break;
                    case "check_plate":
                        parkingLot.CheckPlate();
                        break;
                    case "check_color":
                        Console.Write("$ Enter color of the vehicle: ");
                        string color = Console.ReadLine();
                        parkingLot.CheckColor(color);
                        break;
                    case "match_plate":
                        Console.Write("$ Enter the police plate to check: ");
                        string plate = Console.ReadLine();
                        parkingLot.MatchPlateWithinSlot(plate);
                        break;
                    default:
                        Console.WriteLine("Unknown command. Try again.");
                        break;
                }

                Console.WriteLine();
            }
        }
	}
}
