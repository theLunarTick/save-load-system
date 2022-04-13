using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "word";



    // DATA SOURCE INFROMATION COLLECTED

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption) 
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }



    // LOAD
  
    public GameData Load() 
    {
        // PATH.COMBINE  --  use Path.Combine to account for different OS's having different path separators
        //
        //

        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;


        // DOES FILE EXIST
        //
        //

        if (File.Exists(fullPath)) 
        {
            Debug.Log(fullPath);
            try 
            {
                // LOAD SERIALIZED DATA FROM FILE  --  load the serialized data from the file
                //
                //

                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {

                    //
                    //

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }

                    //
                    //
                }

                // USE DECRYPTION  --  optionally decrypt the data
                //
                //

                if (useEncryption) 
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }


                // DESERIALIZE DATA FROM JSON  --  deserialize the data from Json back into the C# object
                //
                //

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

                Debug.Log(loadedData);
            }

            // 
            //
            //

            catch (Exception e) 
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }

        // LOAD DATA
        //
        //

        return loadedData;
    }



    // SAVE

    public void Save(GameData data) 
    {

        // PATH.COMBINE  --  use Path.Combine to account for different OS's having different path separators
        //
        //

        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try 
        {

            // CREATE DIRECTORY  --  create the directory the file will be written to if it doesn't already exist
            //
            //

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));



            // SERIALIZE TO JSON  --  serialize the C# game data object into Json
            //
            //

            string dataToStore = JsonUtility.ToJson(data, true);


            
            // ENCRYPTION IF MARKED   --  optionally encrypt the data
            //
            //

            if (useEncryption) 
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }



            // SERIALIZE DATA TO FILE  -- write the serialized data to the file
            //
            //

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {


                using (StreamWriter writer = new StreamWriter(stream)) 
                {
                    writer.Write(dataToStore);
                }


            }
        }


        catch (Exception e) 
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }



    // ENCRYPTION  --  the below is a simple implementation of XOR encryption

    private string EncryptDecrypt(string data) 
    {
        string modifiedData = "";



        for (int i = 0; i < data.Length; i++) 
        {
            modifiedData += (char) (data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }



        return modifiedData;
    }
}
