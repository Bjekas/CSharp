using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Globalization;

class PlcManager
{
    #region global_objects
    //Objects
    private List<LNDConnection> plcUnits = new List<LNDConnection>();
    private static LowLevelConversions LLC = new LowLevelConversions();

    //Variables
    private byte[] byteBuffer = new byte[4];

    #endregion
    public PlcManager() 
    { 
           
    }

    #region Private_Functions
    private static byte[] setBitOnByte(int bitNumber, byte[] byteArray)
    {
        BitArray myBA = new BitArray(BitConverter.GetBytes(byteArray[0]).ToArray());
        myBA[bitNumber] = true;
        myBA.CopyTo(byteArray, 0);        

        return byteArray;
    }

    private static byte[] resetBitOnByte(int bitNumber, byte[] byteArray)
    {
        BitArray myBA = new BitArray(BitConverter.GetBytes(byteArray[0]).ToArray());
        myBA[bitNumber] = false;
        myBA.CopyTo(byteArray, 0);

        return byteArray;
    }

    private static byte[] toggleBitOnByte(int bitNumber, byte[] byteArray)
    {
        BitArray myBA = new BitArray(BitConverter.GetBytes(byteArray[0]).ToArray());
        myBA[bitNumber] = !myBA[bitNumber];
        myBA.CopyTo(byteArray, 0);

        return byteArray;
    }

    private LNDConnection findPlcUnit(String plcName)
    {
        foreach (LNDConnection o in plcUnits)
        {
            if (o.getPlcName().CompareTo(plcName) == 0)
                return o;
        }

        return null;
    }

    #endregion

    #region Public_Functions

    #region Units_functions
    public int addPlcUnit(String plcName, String plcIp, int plcSlot)
    {
        if (!containsPlcName(plcName))
            plcUnits.Add(new LNDConnection(plcName, plcIp, plcSlot));
        else
            return -1;
        return 0;
    }

    public int deletePlcUnit(String plcName)
    {
        foreach (LNDConnection o in plcUnits)
        {
            if (o.comparePlcName(plcName))
            {
                o.closeConnection();
                plcUnits.Remove(o);
            }
        }
        return 0;
    }

    public bool initPlcComm(String plcName)
    {
        foreach (LNDConnection o in plcUnits)
        {
            if (o.getPlcName().CompareTo(plcName) == 0)
            {
                o.serialInterfaceInit();

                o.interfaceInit();

                o.connectionInit();

                return true;
            }
        }            

        return false;
    }

    public bool closePlcComm(String plcName)
    {
        foreach (LNDConnection o in plcUnits)
        {
            if (o.getPlcName().CompareTo(plcName) == 0)
            {
                o.closeConnection();

                return true;
            }
        }

        return false;
    }

    public bool containsPlcName(String plcName)
    {
        foreach (LNDConnection o in plcUnits)
        {
            if (o.comparePlcName(plcName))
                return true;                    
        }
            
        return false;
    }

    #endregion

    #region bit_Functions
    public bool setBitOnTag(String plcName, int dbNumber, int byteNumber, int bitNumber)
    {
        foreach (LNDConnection o in plcUnits)
        {
            if (o.getPlcName().CompareTo(plcName) == 0)
            {
                //Reads Byte from PLC
                o.DC.readBytes(libnodave.daveDB, dbNumber, byteNumber, 1, byteBuffer);
                //Changes bit in designated Byte
                byteBuffer = setBitOnByte(bitNumber, byteBuffer);
                //Writes changed Byte into PLC
                o.DC.writeBytes(libnodave.daveDB, dbNumber, byteNumber, 1, byteBuffer);

                return true;
            }
        }

        return false;
    }

    public bool resetBitOnTag(String plcName, int dbNumber, int byteNumber, int bitNumber)
    {
            
        foreach (LNDConnection o in plcUnits)
        {
            if (o.getPlcName().CompareTo(plcName) == 0)
            {
                //Reads Byte from PLC
                o.DC.readBytes(libnodave.daveDB, dbNumber, byteNumber, 1, byteBuffer);
                //Changes bit in designated Byte
                byteBuffer = resetBitOnByte(bitNumber, byteBuffer);
                //Writes changed Byte into PLC
                o.DC.writeBytes(libnodave.daveDB, dbNumber, byteNumber, 1, byteBuffer);

                return true;
            }
        }

        return false;
    }

    public bool toggleBitOnTag(String plcName, int dbNumber, int byteNumber, int bitNumber)
    {
        foreach (LNDConnection o in plcUnits)
        {
            if (o.getPlcName().CompareTo(plcName) == 0)
            {
                //Reads Byte from PLC
                o.DC.readBytes(libnodave.daveDB, dbNumber, byteNumber, 1, byteBuffer);
                //Changes bit in designated Byte
                byteBuffer = toggleBitOnByte(bitNumber, byteBuffer);
                //Writes changed Byte into PLC
                o.DC.writeBytes(libnodave.daveDB, dbNumber, byteNumber, 1, byteBuffer);

                return true;
            }
        }

        return false;
    }

    public bool readBitOnTag(String plcName, int dbNumber, int byteNumber, int bitNumber)
    {
        LNDConnection temp;

        //finds specified unit
        temp = findPlcUnit(plcName);
        //reads entire byte                       
        temp.DC.readBytes(libnodave.daveDB, dbNumber, byteNumber, 1, byteBuffer);
        //converts data to bitArray
        BitArray myBA = new BitArray(BitConverter.GetBytes(byteBuffer[0]).ToArray());

        return myBA[bitNumber];
    }

    #endregion

    #region Byte_Functions
    public String readByte2Bin(String plcName, int dbNumber, int byteNumber)
    {
        LNDConnection temp;

        temp = findPlcUnit(plcName);                                                    //finds specified unit
        temp.DC.readBytes(libnodave.daveDB, dbNumber, byteNumber, 1, byteBuffer);       //reads entire byte            
        return LLC.uInt2BinByte(byteBuffer[0]);                                              //returns bit value in string 
    }

    public String readByte2Hex(String plcName, int dbNumber, int byteNumber)
    {
        return LLC.bin2Hex(readByte2Bin(plcName, dbNumber, byteNumber));
    }

    public String readByte2IntS(String plcName, int dbNumber, int byteNumber)
    {
        return LLC.bin2IntS(readByte2Bin(plcName, dbNumber, byteNumber));
    }

    public int readByte2Int(String plcName, int dbNumber, int byteNumber)
    {
        return LLC.bin2Int(readByte2Bin(plcName, dbNumber, byteNumber));
    }

    public bool writeInt2Byte(String plcName, int dbNumber, int byteNumber, int value)
    {
        if(value >= 0 && value <= 255 )
        {
            LNDConnection temp;

            temp = findPlcUnit(plcName);                                                    //finds specified unit                
            Byte[] tempBA = { Convert.ToByte(value) };
            temp.DC.writeBytes(libnodave.daveDB, dbNumber, byteNumber, 1, tempBA);
            return true;
        }

        return false;
    }

    public bool writeIntS2Byte(String plcName, int dbNumber, int byteNumber, string value)
    {
        writeInt2Byte(plcName, dbNumber, byteNumber, Convert.ToInt32(value));

        return true;
    }

    public bool writeHex2Byte(String plcName, int dbNumber, int byteNumber, string value)
    {
        writeInt2Byte(plcName, dbNumber, byteNumber, LLC.hex2Int(value));

        return true;
    }

    public bool writeBin2Byte(String plcName, int dbNumber, int byteNumber, string value)
    {
        writeInt2Byte(plcName, dbNumber, byteNumber, LLC.bin2Int(value));

        return true;
    }
    #endregion

    #region Word_Functions
    public String readWord2Bin(String plcName, int dbNumber, int wrodNumber)
    {
        LNDConnection temp;

        temp = findPlcUnit(plcName);                                                    //finds specified unit
        temp.DC.readBytes(libnodave.daveDB, dbNumber, wrodNumber, 2, byteBuffer);       //reads 2 Bytes

        return LLC.uInt2BinByte(byteBuffer[0]) + LLC.uInt2BinByte(byteBuffer[1]);                 //returns bit value in string
    }

    public String readWord2Hex(String plcName, int dbNumber, int wordNumber)
    {
        return LLC.bin2Hex(readWord2Bin(plcName, dbNumber, wordNumber));
    }
                
    public String readWord2IntS(String plcName, int dbNumber, int byteNumber)
    {
        return LLC.bin2IntS(readWord2Bin(plcName, dbNumber, byteNumber));
    }

    public int readWord2Int(String plcName, int dbNumber, int byteNumber)
    {
        return LLC.bin2Int(readWord2Bin(plcName, dbNumber, byteNumber));
    }

        
    public bool writeBin2Word(String plcName, int dbNumber, int byteNumber, string value)
    {
        //Finds specified Plc Unit
        LNDConnection temp;
        temp = findPlcUnit(plcName);

        if (value.Length == 8)
        {
            //transforms 1 Word into 2 Bytes
            char[] tempCA = value.ToCharArray();
            string firstValue = tempCA[3].ToString() + tempCA[2].ToString() + tempCA[1].ToString() + tempCA[0].ToString();
            string secondValue = tempCA[7].ToString() + tempCA[6].ToString() + tempCA[5].ToString() + tempCA[4].ToString();

            //Inserts 2 Bytes into Byte Array
            Byte[] tempBA = { Convert.ToByte(firstValue), Convert.ToByte(secondValue) };

            //Writes Bytes into PLC
            temp.DC.writeBytes(libnodave.daveDB, dbNumber, byteNumber, 2, tempBA);
                
            return true;
        }

        return false;
    }

    /*
    public bool writeIntS2Byte(String plcName, int dbNumber, int byteNumber, string value)
    {
        writeInt2Byte(plcName, dbNumber, byteNumber, Convert.ToInt32(value));

        return true;
    }

    public bool writeHex2Byte(String plcName, int dbNumber, int byteNumber, string value)
    {
        writeInt2Byte(plcName, dbNumber, byteNumber, LLC.hex2Int(value));

        return true;
    }

    public bool writeBin2Byte(String plcName, int dbNumber, int byteNumber, string value)
    {
        writeInt2Byte(plcName, dbNumber, byteNumber, LLC.bin2Int(value));

        return true;
    }
    */
    #endregion
    #endregion
}
