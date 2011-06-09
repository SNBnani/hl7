using System;

using System.Data.SqlClient;
using System.Collections;
using System.Collections.Specialized;


namespace hl7service
{
	public class PatientInfo
	{
		public string connectionString;
		
		// SQL Patient info
		public string table = "SCAPersona";

		// HL7 v2 Patient info
		
		protected string PATIENT_ID = "PID";
		protected string FIELD_SEPARATOR = "|";

		protected StringDictionary hl7v2PatientInfo = new StringDictionary();

		protected string [] hl7v2PatientInfoKeys = new string [] {
			"PID",
			"PatientID","ExternalID","InternalID","AlternatePatientID","PatientName","MothersMaidenName",
			"DateTimeofBirth","Sex","PatientAlias","Race","PatientAddress","CountyCode","PhoneNumberHome",
			"PhoneNumberBusiness","PrimaryLanguage","MaritalStatus","Religion","PatientAccountNumber",
			"SSNNumber","DriversLicenseNumber","MothersIdentifier","EthnicGroup","BirthPlace",
			"MultipleBirthIndicator","BirthOrder","Citizenship","VeteransMilitaryStatus",
			"Nationality","PatientDeathDateTime","PatientDeathIndicator" };
		
		/*
		protected StringDictionary sqlPatientInfo = new StringDictionary();
		protected string [] sqlPatientInfoKeys = new string [] { 
			"Tipo", "Referencia", "Nombre", "Nombre1", "Apellido1", "Apellido2", "NHC",
			"Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "Field7", "Field8", "Field9", "Field10",
			"Alta" };
		*/
			
		public PatientInfo()
		{
			SQLServerInfo sqlInfo = new SQLServerInfo();
			
			this.connectionString = sqlInfo.getConnectionString();
			
			hl7v2PatientInfo.Clear();
		}
		
		public void fromSQL()
		{
		}
		
		protected void toSQL()
		{		
		}
		
		public string fromSQLtoHL7v2()
		{
			// TODO : this is just sample code!
			/*
			SqlConnection myConnection = new SqlConnection();
			
			myConnection.ConnectionString = this.connectionString;
			
			try 
			{
				myConnection.Open();
			    SqlDataReader myReader = null;
			    SqlCommand    myCommand = new SqlCommand("select * from table", 
			                                             myConnection);
			    myReader = myCommand.ExecuteReader();
			    while(myReader.Read())
			    {
			        Console.WriteLine(myReader["Column1"].ToString());
			        Console.WriteLine(myReader["Column2"].ToString());
			    }
			}
			catch(Exception e)
			{
				Console.WriteLine("PatientInfo: " + e.ToString());
			}
			*/		
			return "";
		}

		public string fromSQLtoHL7v3()
		{
			// TODO

			return "";
		}
		
		public void fromHL7v2toSQL(string text)
		{
			// Parse HL7 v2 Message
			
			// search for PID field
			int first = text.IndexOf(PATIENT_ID);
			
			if (first != -1)
			{
				hl7v2PatientInfo.Clear();
				
				// Read patient info
				
				string [] split = text.Substring(first).Split(new Char [] {'|'});
				
				for (int index = 0; index < hl7v2PatientInfoKeys.Length; index++)
				{
					string s = split[index];
					hl7v2PatientInfo.Add(hl7v2PatientInfoKeys[index],s);
				}
			}
			
			// Now convert it to SQL
			
			string sqlString = "INSERT INTO SCAPersona (Tipo, Nombre, Nombre1, Apellido1, Apellido2, NHC, Alta) VALUES (";
			
			// Tipo
			sqlString += "2,";
			
			// Calculem quins seran els camps Nombre1, Apellido1 i Apellido2
			
			string fullName = hl7v2PatientInfo["PatientName"];
			
			string [] split = fullName.Split(new Char [] {'^'}); // separem per ^. Primer ve el 1r cognom i després el nom.
			
			string apellido1 = split[0];
			string apellido2 = string.Empty;
			if (hl7v2PatientInfo.ContainsKey("MothersMaidenName"))
			{
				apellido2 = hl7v2PatientInfo["MothersMaidenName"];
			}
			string nombre1 = string.Empty;
			for (int i=1; i<split.Length; i++)
			{
				nombre1 += split[i] + " ";
			}
			
			// Nombre = Apellido1 Apellido2, Nombre1
			sqlString += "'";
			if (apellido2.Length > 0)
			{
				sqlString += apellido1 + " " + apellido2 + "," + nombre1;
			}
			else
			{
				sqlString += apellido1 + "," + nombre1;
			}
			sqlString += "',";			
			
			// Nombre1
			sqlString += "'";
			sqlString += nombre1;
			sqlString += "',";
			
			// Apellido1
			sqlString += "'" + apellido1 + "',";
			
			// Apellido2
         	sqlString += "'" + apellido2 + "',";
			
			// NHC
         	sqlString += "'";
			if (hl7v2PatientInfo.ContainsKey("InternalID"))
			{
				sqlString += hl7v2PatientInfo["InternalID"];
			}
         	sqlString += "',";
			
			// Alta
			
			DateTime today = DateTime.Today;
			
			sqlString += "'" + today.ToString("yyyyMMdd") + "');";
			
			store(sqlString);			
		}
		
		protected void store(string sqlString)
		{
			Logger.Debug("PatientInfo connection string: " + this.connectionString);	
			Logger.Debug("PatientInfo SQL Command: " + sqlString);			

			SqlConnection myConnection = new SqlConnection();		
			myConnection.ConnectionString = this.connectionString;

			try 
			{
				myConnection.Open();
				
				SqlCommand myCmd = new SqlCommand(sqlString, myConnection);
				
				myCmd.ExecuteNonQuery();
			}
			catch(Exception e)
			{
				Logger.Fatal("Can't open connection to database server: " + e.ToString());
			}
			
		}
		
		public void fromCSVtoSQL(string text)
		{
			/*
			 * */
			
		}

		public void fromTXTtoSQL(string text)
		{
			// Read patient info
			
			foreach (string line in text)
			{
			string [] split = text.Split(new Char [] {'\t'});
			
			string sqlString = string.Empty;
			
			foreach (string s in split)
			{
			}	
			
			// Tipo Referencia Nombre Nombre1 Apellido1 Apellido2 NHC Field1 Field2 Field3 Field4 Field5 Field6 Field7 Field8 Field9 Field10 Alta
			
			/*
			 * 
 1	0	Slock,Willy Eduard   Dhr. 	Willy Eduard   Dhr. 	Slock		4005181503	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	2011-06-06

			 * */
			
			// Now convert it to SQL
			
			string sqlString = "INSERT INTO SCAPersona (Tipo, Nombre, Nombre1, Apellido1, Apellido2, NHC, Alta) VALUES (";
			
			// Tipo
			sqlString += "2,";
			
			// Calculem quins seran els camps Nombre1, Apellido1 i Apellido2
			
			string fullName = hl7v2PatientInfo["PatientName"];
			
			string [] split = fullName.Split(new Char [] {'^'}); // separem per ^. Primer ve el 1r cognom i després el nom.
			
			string apellido1 = split[0];
			string apellido2 = string.Empty;
			if (hl7v2PatientInfo.ContainsKey("MothersMaidenName"))
			{
				apellido2 = hl7v2PatientInfo["MothersMaidenName"];
			}
			string nombre1 = string.Empty;
			for (int i=1; i<split.Length; i++)
			{
				nombre1 += split[i] + " ";
			}
			
			// Nombre = Apellido1 Apellido2, Nombre1
			sqlString += "'";
			if (apellido2.Length > 0)
			{
				sqlString += apellido1 + " " + apellido2 + "," + nombre1;
			}
			else
			{
				sqlString += apellido1 + "," + nombre1;
			}
			sqlString += "',";			
			
			// Nombre1
			sqlString += "'";
			sqlString += nombre1;
			sqlString += "',";
			
			// Apellido1
			sqlString += "'" + apellido1 + "',";
			
			// Apellido2
         	sqlString += "'" + apellido2 + "',";
			
			// NHC
         	sqlString += "'";
			if (hl7v2PatientInfo.ContainsKey("InternalID"))
			{
				sqlString += hl7v2PatientInfo["InternalID"];
			}
         	sqlString += "',";
			
			// Alta
			
			DateTime today = DateTime.Today;
			
			sqlString += "'" + today.ToString("yyyyMMdd") + "');";
			
			store(sqlString);			
			
			
			
			// Each TXT can contain more than one patient info
			
		}
		
		public void fromHL7v3toSQL(string xml)
		{
			// TODO		
			/*
			XmlTextReader reader = new XmlTextReader(fileName);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        Console.Write("<" + reader.Name);
                        while (reader.MoveToNextAttribute())
                            Console.Write(" " + reader.Name + "='" + reader.Value + "'");
                            Console.WriteLine(">");
                        break;

                    case XmlNodeType.Text:
                        Console.WriteLine (reader.Value);
                        break;
                    case XmlNodeType.EndElement: //Mostrar el final del elemento.
                        Console.Write("</" + reader.Name);
                        Console.WriteLine(">");
                        break;
                }
            }
            */
		}
	}
}

