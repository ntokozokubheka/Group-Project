using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using HashPass;

namespace UREX_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        string IService1.GetData(int value)
        {
            throw new NotImplementedException();
        }

        CompositeType IService1.GetDataUsingDataContract(CompositeType composite)
        {
            throw new NotImplementedException();
        }

        bool IService1.RegisterUser(string name, string surname, string email, string idNumber, string phone, string password, string userType)
        {
            // Email already in use
            if (db.Users.Any(u => u.Email == email))
                return false;

            var newUser = new User
            {
                Name = name,
                Surname = surname,
                Email = email,
                IDNumber = idNumber,
                ContactNumber = phone,
                Password = HashPass.Secrecy.HashPassword(password),
                UserType = userType
            };

            db.Users.InsertOnSubmit(newUser);
            try
            {
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return false;
            }
        }

        int IService1.LoginUser(string email, string password)
        {
            var user = (from u in db.Users
                        where u.Email.Equals(email) &&
                        u.Password.Equals(HashPass.Secrecy.HashPassword(password))
                        select u).FirstOrDefault();

            if (user != null)
            {
                return user.UserId;
            }

            return -1;
        }

        public User getUser(int id)
        {
            User user = (from u in db.Users
                         where u.UserId.Equals(id)
                         select u).FirstOrDefault();

            return user;
        }

        public bool checkIfRegistered(string email, string password)
        {

            var user = (from u in db.Users
                              where u.Email.Equals(email) && u.Password.Equals(password)
                              select u).FirstOrDefault();

            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
