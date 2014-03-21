﻿
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Hik.Communication.ScsServices.Service;

namespace PhoneBookCommonLib
{
    /// <summary>
    /// This interface defines methods of Phone Book Service
    /// that can be called remotely by client applications.
    /// </summary>
    [ScsService(Version = "1.0.0.0")]
    public interface IPhoneBookService : INotifyPropertyChanged
    {

        string Title { get; set; }

        /// <summary>
        /// Adds a new person to phone book.
        /// </summary>
        /// <param name="recordToAdd">Person informations to add</param>
        void AddPerson(PhoneBookRecord recordToAdd);

        /// <summary>
        /// Deletes a person from phone book.
        /// </summary>
        /// <param name="name">Name of the person to delete</param>
        /// <returns>True, if a person is deleted,
        ///          false if person is not found</returns>
        bool DeletePerson(string name);

        /// <summary>
        /// Searches a person in phone book by name of person.
        /// </summary>
        /// <param name="name">Name of person to search.
        /// Name might not fully match, it can be a part of person's name</param>
        /// <returns>Person informations if found, else null</returns>
        [Cacheable]
        PhoneBookRecord FindPerson(string name);
    }
}
