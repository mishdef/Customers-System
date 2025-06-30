using MyFunctions;
using System;
using System.IO;
using System.Linq;
using System.Text;
using static MyFunctions.Tools;

namespace ConsoleApp1
{
    internal class Program
    {
        public static Customer[] customers = new Customer[0];

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string fileName;

            try
            {
                string prevFile = "";
                using (FileStream file = new FileStream("settings.bin", FileMode.Open))
                using (BinaryReader read = new BinaryReader(file))
                {
                    try
                    {
                        while (true)
                        {
                            prevFile = read.ReadString();
                        }
                    }
                    catch (EndOfStreamException ex) { }
                }

                if (prevFile == "")
                {
                    Console.WriteLine();
                    fileName = InputFileName("Input filename (filename must end with \".bin\") --> ", ".bin");
                }
                else
                {
                    Console.WriteLine(
                        $"Do you want to open the previous file ({prevFile})? \n\tIf you want, press Enter. \n\tIf you don't, enter a new filename. \n\t(If it exists, it will be opened. If it doesn't exist, it will be created.)"
                    );
                    fileName = inputFileName("", prevFile);
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(LoadCustomersFromFile(fileName));
                }
                else
                {
                    using (FileStream create = new FileStream(fileName, FileMode.CreateNew)) ;
                    Console.WriteLine($"File {fileName} is created.");
                }
                using (FileStream file = new FileStream("settings.bin", FileMode.Open))
                using (BinaryWriter write = new BinaryWriter(file))
                {
                    write.Write(fileName);
                }

                do
                {
                    try
                    {
                        Console.Clear();
                        Console.WriteLine("+=== Main Menu ===+");
                        Console.WriteLine("1. Add new customer");
                        Console.WriteLine("2. Search");

                        switch (InputInt("Input number --> "))
                        {
                            case 1:
                                {
                                    NewCustomerMenu(fileName);
                                    break;
                                }
                            case 2:
                                {
                                    SearchWithNameMenu(fileName);
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine("WARNING | Invalid menu item. Try again...");
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}");
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        static void DellCustomer(Customer cust, string fileName)
        {
            int idToRemove = -1;
            for (int i = 0; i < customers.Length; i++)
            {
                if (cust == customers[i])
                {
                    idToRemove = i;
                    break;
                }
            }

            if (idToRemove == -1)
            {
                Console.WriteLine("Ошибка: Указанный клиент для удаления не найден в списке.");
                return;
            }

            for (int i = idToRemove; i < customers.Length - 1; i++)
            {
                customers[i] = customers[i + 1];
            }

            Array.Resize(ref customers, customers.Length - 1);

            AcceptChanges(fileName);
        }

        static string inputFileName(string text, string prewFilename)
        {
            string fileName = "";

            do
            {
                Console.Write("--> ");
                fileName = Console.ReadLine();
                if (fileName == "")
                    return prewFilename;
                if (!(fileName.EndsWith(".bin")))
                {
                    Console.WriteLine("File name must end with \".bin\". Please try again.");
                }
            } while (!(fileName.EndsWith(".bin")));
            return fileName;
        }

        static string LoadCustomersFromFile(string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            using (BinaryReader read = new BinaryReader(file))
            {
                if (file.Length == 0)
                    return $"WARNING | File exists but it's empty.";

                string text = "";
                try
                {
                    while (true)
                    {
                        text += read.ReadString();
                    }
                }
                catch (EndOfStreamException) { }
                catch (Exception ex)
                {
                    return $"ERROR | An error occurred while reading the file: {ex.Message}";
                }

                string[] customersInfo = text.Split('þ').Where(s => !string.IsNullOrEmpty(s)).ToArray();

                int i = 0;
                foreach (var customerInfoString in customersInfo)
                {
                    Array.Resize(ref customers, customers.Length + 1);
                    customers[customers.Length - 1] = new Customer(customerInfoString);
                    i++;
                }

                return $"Loaded {i} customers.";
            }
        }

        static void SearchWithNameMenu(string fileName)
        {
            do
            {
                try
                {
                    string prompt = "";
                    Customer[] foundedCustomers = new Customer[0];
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("Input prompt (input '/' to show all customers)--> ");
                        prompt = Console.ReadLine();
                        if (prompt == "exit")
                            return;
                        if (prompt != "")
                            break;
                    } while (true);

                    foreach (Customer customer in customers)
                    {
                        if (customer.ToFileText().Contains(prompt))

                        {
                            Array.Resize(ref foundedCustomers, foundedCustomers.Length + 1);
                            foundedCustomers[foundedCustomers.Length - 1] = customer;
                        }
                    }

                    int choose;

                    if (foundedCustomers.Length == 0)
                    {
                        MyFunctions.MessageBox.Show("No results found.", "Message", MessageBox.Buttons.Ok);
                        return;
                    }

                    if (foundedCustomers.Length == 1)
                    {
                        choose = 1;

                        Console.WriteLine(customers[choose - 1].ToString() + "\n");

                        Console.WriteLine("Press enter to edit or type \"exit\" to exit");
                        string res = Console.ReadLine();
                        if (res == "exit")
                        {
                            return;
                        }
                        else if (res == "") { }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Found {foundedCustomers.Length} customers");
                        int i = 1;
                        foreach (Customer customer in foundedCustomers)
                        {
                            DrawLine(foundedCustomers[0].ToString().Length);
                            Console.WriteLine(i + ". " + customer.ToString() + "\n");
                            i++;
                        }
                        DrawLine(foundedCustomers[0].ToString().Length);

                        choose = InputInt("Choose customer to edit (input number) --> ", InputType.With, 1, i - 1);
                    }

                    try
                    {
                        do
                        {
                            Console.Clear();
                            DrawLine(foundedCustomers[choose - 1].ToString().Length);
                            Console.WriteLine(foundedCustomers[choose - 1].ToString());
                            DrawLine(foundedCustomers[choose - 1].ToString().Length);
                            Console.WriteLine("+=== Customer Menu ===+");
                            Console.WriteLine("1. Add service");
                            Console.WriteLine("2. Edit customer's information");
                            Console.WriteLine("3. Delete customer");
                            Console.WriteLine("4. Delete service");
                            Console.WriteLine("5. Main menu");

                            switch (InputInt("Input number --> "))
                            {
                                case 1:
                                    {
                                        AddService(fileName, foundedCustomers[choose - 1]);
                                        break;
                                    }
                                case 2:
                                    {
                                        EditCustomerInfo(fileName, foundedCustomers[choose - 1]);
                                        break;
                                    }
                                case 3:
                                    {
                                        Console.WriteLine("Are you sure? (y/n)\n");
                                        Console.CursorVisible = false;
                                        ConsoleKeyInfo key = Console.ReadKey();
                                        Console.CursorVisible = true;
                                        if (key.KeyChar == 'y' || key.KeyChar == 'Y')
                                        {
                                            DellCustomer(foundedCustomers[choose - 1], fileName);
                                            AcceptChanges(fileName);

                                            Console.WriteLine($"\nCustomer is deleted.");
                                            return;
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        if (foundedCustomers[choose - 1].Services == null || foundedCustomers[choose - 1].Services.Length == 0 || foundedCustomers[choose - 1].Services[0].Length == 0)
                                        {
                                            Console.WriteLine("No services to delete.");
                                            Console.WriteLine("Press Enter to continue...");
                                            Console.ReadLine();
                                            break;
                                        }
                                        else
                                        {
                                            DisplayCustomerServices(foundedCustomers[choose - 1]);
                                            int serviceIndex = InputInt("Input service number to delete --> ", InputType.With, 1, foundedCustomers[choose - 1].Services[0].Length);
                                            Console.WriteLine("Are you sure? (y/n)\n");
                                            Console.CursorVisible = false;
                                            ConsoleKeyInfo key = Console.ReadKey();
                                            Console.CursorVisible = true;
                                            if (key.KeyChar == 'y' || key.KeyChar == 'Y')
                                            {
                                                foundedCustomers[choose - 1].DeleteService(serviceIndex - 1);
                                                AcceptChanges(fileName);
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nDeleting service is canceled. Coming back to main menu...");
                                            }
                                        }
                                        break;
                                    }
                                case 5:
                                    {
                                        return;
                                    }
                                default:
                                    {
                                        Console.WriteLine("WARNING | Invalid menu item. Try again...");
                                        break;
                                    }
                            }
                        } while (true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } while (true);
        }

        static void DisplayCustomerServices(Customer customer)
        {
            string[][] services = customer.Services;

            if (services == null || services.Length != 3 || services[0].Length == 0)
            {
                Console.WriteLine($"No services recorded for customer: {customer.Name}");
                return;
            }

            Console.WriteLine($"─────────── Services for {customer.Name} ────────────");
            for (int i = 0; i < services[0].Length; i++)
            {
                string date = services[0][i];
                string description = services[1][i];
                string cost = services[2][i];
                Console.WriteLine($"{i + 1}. Date: {date}, Description: {description}, Cost: {cost}");
            }
            Console.WriteLine("──────────────────────────────────────────────────────");
        }

        static void AddService(string fileName, Customer customer)
        {
            try
            {
                bool end = false;
                string date = "";
                string desc = "";
                double cost = 0;

                int i = 0;
                do
                {
                    string temp = "";
                    Console.Clear();
                    Console.WriteLine($"Date: {date}");
                    Console.WriteLine($"Description: {desc}");
                    Console.WriteLine($"Cost: {cost}");

                    switch (i)
                    {
                        case 0:
                            {
                                Console.Write("Input date (HH:mm, DD.MM.YYYY) (or press Enter to skip)--> ");
                                temp = Console.ReadLine();
                                if (temp != "")
                                {
                                    date = temp;
                                }
                                i++;
                                break;
                            }
                        case 1:
                            {
                                Console.Write("Input description (or press Enter to skip)--> ");
                                temp = Console.ReadLine();
                                if (temp != "")
                                {
                                    desc = temp;
                                }
                                i++;
                                break;
                            }
                        case 2:
                            {
                                double tempDouble = InputDouble("Input cost (or press Enter to skip)--> ");
                                cost = tempDouble;
                                i++;
                                break;
                            }
                        case 3:
                            {
                                Console.WriteLine("Everything is correct? (y/n)\nPress (c) to cancel.");
                                Console.CursorVisible = false;
                                ConsoleKeyInfo key = Console.ReadKey();
                                Console.CursorVisible = true;
                                if (key.KeyChar == 'y' || key.KeyChar == 'Y')
                                {
                                    customer.AddService(date, desc, cost);

                                    AcceptChanges(fileName);

                                    Console.WriteLine($"\nNew service is added.");
                                    end = true;
                                }
                                else if (key.KeyChar == 'c' || key.KeyChar == 'C')
                                {
                                    Console.WriteLine("\nAdding service is canceled. Coming back to main menu...");
                                    return;
                                }
                                else
                                {
                                    i = 0;
                                    date = "";
                                    desc = "";
                                    cost = 0;
                                }
                                break;
                            }
                    }
                } while (!end);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        static void AcceptChanges(string fileName)
        {
            File.Delete(fileName);
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var bw = new BinaryWriter(fileStream))
            {
                foreach (var cust in customers)
                {
                    bw.Write(cust.ToFileText());
                }
            }
        }

        static void EditCustomerInfo(string fileName, Customer customer)
        {
            try
            {
                bool end = false;

                string name = customer.Name;
                string phone = customer.PhoneNumber;
                string adress = customer.Address;

                int i = 0;
                do
                {
                    string temp = "";
                    Console.Clear();
                    Console.WriteLine($"Name: {name}");
                    Console.WriteLine($"Phone: {phone}");
                    Console.WriteLine($"Address: {adress}");

                    switch (i)
                    {
                        case 0:
                            {
                                Console.Write("Input edited name (or press Enter to skip)-->");
                                temp = Console.ReadLine();
                                if (temp != "")
                                {
                                    name = temp;
                                }
                                i++;
                                break;
                            }
                        case 1:
                            {
                                Console.Write("Input edited phone number (or press Enter to skip)-->");
                                temp = Console.ReadLine();
                                if (temp != "")
                                {
                                    phone = temp;
                                }
                                i++;
                                break;
                            }
                        case 2:
                            {
                                Console.Write("Input edited address (or press Enter to skip)-->");
                                temp = Console.ReadLine();
                                if (temp != "")
                                {
                                    adress = temp;
                                }
                                i++;
                                break;
                            }
                        case 3:
                            {
                                Console.WriteLine("Everything is correct? (y/n)\nPress (c) to cancel.");
                                Console.CursorVisible = false;
                                ConsoleKeyInfo key = Console.ReadKey();
                                Console.CursorVisible = true;
                                if (key.KeyChar == 'y' || key.KeyChar == 'Y')
                                {
                                    customer.Name = name;
                                    customer.PhoneNumber = phone;
                                    customer.Address = adress;

                                    AcceptChanges(fileName);

                                    Console.WriteLine($"\nNew customer \"{name}\" is edited.");
                                    end = true;
                                }
                                else if (key.KeyChar == 'c' || key.KeyChar == 'C')
                                {
                                    Console.WriteLine("\nEditing customer is canceled. Coming back to main menu...");
                                    return;
                                }
                                else
                                {
                                    i = 0;
                                    name = customer.Name;
                                    phone = customer.PhoneNumber;
                                    adress = customer.Address;
                                }
                                break;
                            }
                    }
                } while (!end);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        static void NewCustomerMenu(string fileName)
        {
            try
            {
                bool end = false;
                string name = "", phone = "", adress = "";
                int i = 0;
                do
                {
                    Console.Clear();
                    Console.WriteLine($"Name: {name}");
                    Console.WriteLine($"Phone: {phone}");
                    Console.WriteLine($"Address: {adress}");

                    switch (i)
                    {
                        case 0:
                            {
                                Console.Write("Input name -->");
                                name = Console.ReadLine();
                                i++;
                                break;
                            }
                        case 1:
                            {
                                Console.Write("Input phone number -->");
                                phone = Console.ReadLine();
                                i++;
                                break;
                            }
                        case 2:
                            {
                                Console.Write("Input address (if don't want just press Enter)-->");
                                adress = Console.ReadLine();
                                if (adress == "")
                                {
                                    adress = "-";
                                }
                                i++;
                                break;
                            }
                        case 3:
                            {
                                Console.WriteLine("Everything is correct? (y/n)\nPress (c) to cancel.");
                                Console.CursorVisible = false;
                                ConsoleKeyInfo key = Console.ReadKey();
                                Console.CursorVisible = true;
                                if (key.KeyChar == 'y' || key.KeyChar == 'Y')
                                {
                                    Array.Resize(ref customers, customers.Length + 1);

                                    customers[customers.Length - 1] = new Customer(name, phone, adress);
                                    using (var fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.None))
                                    using (var bw = new BinaryWriter(fileStream))
                                    {
                                        bw.Write(customers[customers.Length - 1].ToFileText());
                                    }

                                    Console.WriteLine($"\nNew customer \"{name}\" is added.");
                                    end = true;
                                }
                                else if (key.KeyChar == 'c' || key.KeyChar == 'C')
                                {
                                    Console.WriteLine("\nCreating new customer is canceled. Coming back to main menu...");
                                    return;
                                }
                                else
                                {
                                    i = 0;
                                    name = "";
                                    phone = "";
                                    adress = "";
                                }
                                break;
                            }
                    }
                } while (!end);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}