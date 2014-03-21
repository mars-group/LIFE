using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using PhoneBookCommonLib;

namespace PhoneBookServer
{
    /// <summary>
    /// This class implements Phone Book Service contract.
    /// </summary>
    class PhoneBookService : ScsService, IPhoneBookService
    {
        /// <summary>
        /// Current records that are added to phone book service.
        /// Key: Name of the person.
        /// Value: PhoneBookRecord object.
        /// </summary>
        private readonly SortedList<string, PhoneBookRecord> _records;

        private IScsServiceApplication server;
        private string _title;

        /// <summary>
        /// Creates a new PhoneBookService object.
        /// </summary>
        public PhoneBookService()
        {
            _records = new SortedList<string, PhoneBookRecord>();
            //Create a Scs Service application that runs on 10048 TCP port.
            server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(10048));
            
            //Add Phone Book Service to service application
            server.AddService<IPhoneBookService, PhoneBookService>(this);

            //Start server
            server.Start();
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value != Title)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }

            }
        }

        /// <summary>
        /// Adds a new person to phone book.
        /// </summary>
        /// <param name="recordToAdd">Person informations to add</param>
        public void AddPerson(PhoneBookRecord recordToAdd)
        {
            if (recordToAdd == null)
            {
                throw new ArgumentNullException("recordToAdd");
            }

            _records[recordToAdd.Name] = recordToAdd;
        }

        /// <summary>
        /// Deletes a person from phone book.
        /// </summary>
        /// <param name="name">Name of the person to delete</param>
        /// <returns>True, if a person is deleted,
        ///     false if person is not found</returns>
        public bool DeletePerson(string name)
        {
            if (!_records.ContainsKey(name))
            {
                return false;
            }

            _records.Remove(name);
            return true;
        }

        /// <summary>
        /// Searches a person in phone book by name of person.
        /// </summary>
        /// <param name="name">Name of person to search.
        /// Name might not fully match, it can be a part of person's name</param>
        /// <returns>Person informations if found, else null</returns>
        public PhoneBookRecord FindPerson(string name)
        {
            //Get recods by name if there is a record exactly match to given name
            if (_records.ContainsKey(name))
            {
                return _records[name];
            }

            //Search all records to check if there
            //is a name string that contains given name
            foreach (var record in _records)
            {
                if (record.Key.ToLower().Contains(name.ToLower()))
                {
                    return record.Value;
                }
            }

            //Not found
            return null;
        }

        public void Stop()
        {
            server.Stop();
        }


        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
