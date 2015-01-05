using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LowLevelConversions
{
    public int bin2Int(string value)
    {
        return Convert.ToInt32(value, 2);
    }

    public int hex2Int(string value)
    {
        // strip the leading 0x
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            value = value.Substring(2);

        return Convert.ToInt32(value, 16);
    }

    public string bin2IntS(string value)
    {
        return Convert.ToInt32(value, 2).ToString();
    }

    public String hex2IntS(string value)
    {
        // strip the leading 0x
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return value.Substring(2);

        return Convert.ToInt32(value, 16).ToString();
    }

    public String int2Hex(int value)
    {
        return value.ToString("X");
    }

    public String intS2Hex(string value)
    {
        return Convert.ToInt32(value).ToString("X");
    }

    public String bin2Hex(string value)
    {
        return Convert.ToInt32(bin2IntS(value)).ToString("X");
    }

    public string uInt2BinByte(int value)
    {
        if (value >= 0 && value < 256)
            return Convert.ToString(value, 2).PadLeft(8, '0');
        else
            return null;
    }

    public string uIntS2BinByte(string value)
    {
        int temp = Convert.ToInt32(value);
        if (temp >= 0 && temp < 256)
            return Convert.ToString(Convert.ToInt32(value), 2).PadLeft(8, '0');
        else
            return null;
    }

    public string hex2BinByte(string value)
    {
        int temp = Convert.ToInt32(value);

        if (temp >= 0 && temp < 256)
            return Convert.ToString(hex2Int(value), 2).PadLeft(8, '0');
        else
            return null;
    }

    public string uInt2BinWord(int value)
    {
        if (value >= 0 && value < 65535)
            return Convert.ToString(value, 2).PadLeft(16, '0');
        else
            return null;
    }

    public string uIntS2BinWord(string value)
    {
        int temp = Convert.ToInt32(value);
            
        if (temp >= 0 && temp < 65535)
            return Convert.ToString(Convert.ToInt32(value), 2).PadLeft(16, '0');
        else
            return null;
    }

    public string hex2BinWord(string value)
    {
        int temp = Convert.ToInt32(value);

        if (temp >= 0 && temp < 65535)
            return Convert.ToString(hex2Int(value), 2).PadLeft(16, '0');
        else
            return null;
    }

    public string swapWordBytes(string value)
    {
        Byte[] tempBA = new Byte[2];
        Char[] tempCA = new Char[16];
        String tempString = "";
        value = value.PadLeft(16, '0');

        if (value.Length == 16)
        {    
            tempCA = value.ToCharArray();

            for (int i = 8; i <16; i++)
                tempString = tempString + tempCA[i];
            tempBA[0] = Convert.ToByte(bin2Int(tempString));

            tempString = "";

            for (int i = 0; i < 8; i++)
                tempString = tempString + tempCA[i];
            tempBA[1] = Convert.ToByte(bin2Int(tempString));

            return uInt2BinByte(tempBA[0]) + uInt2BinByte(tempBA[1]);
        }
        else
            return null;
    }
}
