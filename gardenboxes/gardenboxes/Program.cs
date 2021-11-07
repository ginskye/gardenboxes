using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace gardenboxes
{
    class Program
    {
        static void Main(string[] args)
        {

            //Write a program that will take in a users garden box size, then let them pick from a list of plants and tell them how many they can plant in that space.

            //Please create a database that will hold plants. You don't need to add more than 2 or 3 plants into the database for testing. Please make sure the database or a description of the database is included in your repo.
            SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\AcademyPgh\source\gits\garden-boxes\gardenboxes\gardenboxes\Database1.mdf;Integrated Security=True");
            connection.Open();

            bool again = true;
            while (again == true)
            {
                Console.WriteLine("Welcome to the Garden Box program! This program will calculate your garden area, and show how many of each type of plant you can fit in this area");
                Console.WriteLine("Would you like to c)reate a garden, a)dd a plant or ex)it?");
                string choice = Console.ReadLine().ToLower();

                if (choice == "c")
                {
                    MakeGarden(connection);


                    bool loopagain = true;
                    while (loopagain == true)
                    {
                        Console.WriteLine("Here is a list of plants in our database");
                        ShowAllPlants(connection);

                        Console.WriteLine("Please enter the ID (as a number) of a vegetable you would like to plant");
                        Globalvars.vegchoice = Convert.ToInt32(Console.ReadLine());

                        ShowPlant(Globalvars.area, Globalvars.vegchoice, connection);

                        UserPlants(Globalvars.vegchoice, Globalvars.gardenerid, Globalvars.yourplants, connection);

                        Console.WriteLine("Would you like to plant another vegetable? Please answer y/n");
                        string moreveg = Console.ReadLine().ToLower();
                        if (moreveg != "y")
                        {
                            loopagain = false;
                        }
                    }

                }

                else if (choice == "a")
                {
                    Console.WriteLine("Enter the name of the plant you'd like to add to the database");
                    string newplant = Console.ReadLine();
                    Console.WriteLine("Enter the amount of space this plant takes up, in square feet.  For example, carrots have a value of 1 per square foot, while corn has a value of 0.1875");
                    decimal newarea = Convert.ToDecimal(Console.ReadLine());

                    Addplant(newplant, newarea, connection);

                }

                else if (choice =="x")
                {
                    Console.WriteLine("Thanks for using the garden box program");
                    again = false;
                }

                else
                {
                    Console.WriteLine("That isn't one of the available options. Please try again.");
                }
            }
            connection.Close();

        }
        public static void Addplant(string string1, decimal nums1, SqlConnection connection)
        {
            SqlCommand insertplant = new SqlCommand($"INSERT INTO Plants (Plant, plantspersqft) VALUES ('{string1}', {nums1})", connection);
            insertplant.ExecuteNonQuery();
        }

        public static void UserPlants(int plantid, int gardenerid, int yourplants, SqlConnection connection)
        {
            SqlCommand userplants = new SqlCommand($"INSERT INTO PlantChoice(PlantsId, GardenerId, numbercanplant) VALUES ({plantid}, {gardenerid}, {yourplants})", connection);
            userplants.ExecuteNonQuery();
        }

        public static void ShowAllPlants(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand("SELECT * from Plants", connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
               Console.WriteLine($"ID:{reader["Id"]}-{reader["Plant"]}");

            }
            reader.Close();
        }

        public static int Area(int num1, int num2)
        {
            return (num1 * num2);
        }

        
        public static void ShowPlant(int area, int plantid, SqlConnection connection)
        {
            SqlCommand showplant = new SqlCommand($"SELECT * from Plants WHERE ID={plantid}", connection);
            SqlDataReader readagain = showplant.ExecuteReader();
            if (readagain.HasRows)
                {
                while (readagain.Read())
                {
                    Globalvars.plantnum = Convert.ToDouble(readagain["plantspersqft"]);
                    Globalvars.yourplants = (int)Math.Floor(area * Globalvars.plantnum);
                    Console.WriteLine($"You can fit {Globalvars.yourplants} {readagain["Plant"]} in your {area} Square Foot Garden");

                }
            }
            else
            {
                Console.WriteLine("That plant id is not in the database");
            }
            readagain.Close();
        }

        public static void MakeGarden(SqlConnection connection)
        {
            Console.WriteLine("Please enter your name");
            Globalvars.username = Console.ReadLine();
            Console.WriteLine("Tell me the length of your garden bed in feet");
            int length = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Tell me the width of your garden bed in feet");
            int width = Convert.ToInt32(Console.ReadLine());
            Globalvars.area = Area(length, width);
            Console.WriteLine($"The area of your garden is {Globalvars.area} square feet");

            SqlCommand userinfo = new SqlCommand($"INSERT INTO [GardenBox](Gardener, boxsize) VALUES ('{Globalvars.username}', {Globalvars.area}); SELECT @@IDENTITY AS Id", connection);
            SqlDataReader userread = userinfo.ExecuteReader();
            userread.Read();
            Globalvars.gardenerid = Convert.ToInt32(userread["Id"]);
            userread.Close();
        }
        
        public static class Globalvars
        {
            public static int area = 0;
            public static int yourplants = 0;
            public static double plantnum = 0;
            public static int gardenerid = 0;
            public static int vegchoice = 0;
            public static string username = "";
        }
    }
}

