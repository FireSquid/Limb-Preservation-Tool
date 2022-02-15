using LimbPreservationTool.Models;
using LimbPreservationTool.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
namespace LimbPreservationTool.Models
{
    public class User
    {
		private static User instance;
        readonly String ID;
		readonly String password;
        List<Patient> PatientList;		
        private User(String user_id, String user_password)
        {
            ID = user_id;
			password = user_password;
			
        }


	    public static async Task<User> CreateInstance( String user_id, String user_password){
			instance =  new User(user_id,user_password);	
			Console.WriteLine($"user model created \n{user_id} \n{user_password}");
		    return instance;	 		
		}

		public static User GetInstance(){
		    return instance;	 		
		}
		
		public void LogOut(){
			instance = null;			
		}	
    }
}