using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Customer
    {
        string _name;
        string _phoneNumber;
        string _address;

        string[][] _services = new string[3][]
        {
            new string[] { },
            new string[] { },
            new string[] { }
        };

        public string[][] Services { get { return _services; } set { _services = value; }}
        public string Name { get { return _name; } set { _name = value; } }
        public string PhoneNumber { get { return _phoneNumber; } set { _phoneNumber = value; } }
        public string Address { get { return _address; } set { _address = value; } }

        public string AddService(string date, string description, double cost)
        {
            try
            {
                int currentServiceCount = _services[0].Length;
                Array.Resize(ref _services[0], currentServiceCount + 1);
                Array.Resize(ref _services[1], currentServiceCount + 1);
                Array.Resize(ref _services[2], currentServiceCount + 1);

                _services[0][currentServiceCount] = date;
                _services[1][currentServiceCount] = description;
                _services[2][currentServiceCount] = cost.ToString();

                return "Success! New service added!";
            }catch (Exception ex) { return $"Error in adding new service to customer: {ex.Message}"; }
        }

        public bool DeleteService(int serviceIndex)
        {
            int currentServiceCount = _services[0].Length;

            if (serviceIndex < currentServiceCount - 1)
            {
                for (int i = serviceIndex; i < currentServiceCount - 1; i++)
                {
                    _services[0][i] = _services[0][i + 1];
                    _services[1][i] = _services[1][i + 1];
                    _services[2][i] = _services[2][i + 1];
                }
            }

            Array.Resize(ref _services[0], currentServiceCount - 1);
            Array.Resize(ref _services[1], currentServiceCount - 1);
            Array.Resize(ref _services[2], currentServiceCount - 1);

            return true;
        }

        public string ToFileText()
        {
            string text = $"{_name}/{_phoneNumber}/{_address}";

            if (_services != null && _services.Length == 3 && _services[0].Length > 0)
            {
                for (int i = 0; i < _services[0].Length; i++)
                {
                    text += $"/{_services[0][i]}/{_services[1][i]}/{_services[2][i]}";
                }
            }
            return text + "þ";
        }

        public override string ToString()
        {
            string text = $"Name: {_name}\nPhone number: {_phoneNumber}\nAddress: {_address}\n";
            foreach (var col in _services)
            {
                foreach (var row in col)
                {
                    text += row.ToString();
                    text += "\t|\t";
                }
            }
            return text;
        }

        public Customer() { }

        public Customer(string text) 
        {
            string[] s = text.Split('/');

            _name = s[0];
            _phoneNumber = s[1];
            _address = s[2];

            for (int i = 3; i < s.Length; i += 3)
            {
                AddService(s[i], s[i+1], double.Parse(s[i+2]));
            }
        }

        public Customer(string name, string phoneNumber, string address)
        {
            _name = name;
            _phoneNumber = phoneNumber;
            _address = address;
        }
    }
}
