using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LNDConnection
{
    public libnodave.daveOSserialType DST;
    public libnodave.daveInterface DI;
    public libnodave.daveConnection DC;

    private String plcName, plcIp;
    private int plcRack, plcSlot, plcPort;

    #region Class_Constructor
    public LNDConnection(String plcName, String plcIp, int plcSlot)
    {
        this.plcName = plcName;
        this.plcIp = plcIp;
        this.plcPort = 102;
        this.plcRack = 0;
        this.plcSlot = plcSlot;
    }

    public LNDConnection(String plcName, String plcIp, int plcRack, int plcSlot)
    {
        this.plcName = plcName;
        this.plcIp = plcIp;
        this.plcPort = 102;
        this.plcRack = plcRack;
        this.plcSlot = plcSlot;
    }

    public LNDConnection(String plcName, String plcIp, int plcPort, int plcRack, int plcSlot)
    {
        this.plcName = plcName;
        this.plcIp = plcIp;
        this.plcPort = plcPort;
        this.plcRack = plcRack;
        this.plcSlot = plcSlot;
    }
    #endregion

    #region Functions
    public bool serialInterfaceInit()
    {
        DST.rfd = libnodave.openSocket(plcPort, plcIp);
        DST.wfd = DST.rfd;

        if (DST.rfd <= 0)
        {
            Console.WriteLine("Could not initialize SerialType!");
            Console.ReadKey();

            return false;
        }

        return true;
    }

    public bool interfaceInit()
    {
        int res;

        DI = new libnodave.daveInterface(DST, "IF1", 0, libnodave.daveProtoISOTCP, libnodave.daveSpeed187k);
        DI.setTimeout(1000000);
        res = DI.initAdapter();

        if (res != 0)
        {
            Console.WriteLine("Could not initialize Interface!");
            Console.ReadKey();

            return false;
        }
        return true;
    }

    public bool connectionInit()
    {
        int res;

        DC = new libnodave.daveConnection(DI, 0, plcRack, plcSlot);
        res = DC.connectPLC();

        if (res != 0)
        {
            Console.WriteLine("Could not initialize connection to PLC!");
            Console.ReadKey();

            return false;
        }

        return true;
    }

    public void closeConnection()
    {
        if (DC != null)
            DC.disconnectPLC();
        if (DI != null)
            DI.disconnectAdapter();
        
        DST.rfd = libnodave.closeSocket(plcPort);
        DST.wfd = DST.rfd;
    }

    public bool comparePlcName(String plcName)
    {
        if (plcName.CompareTo(this.plcName) == 0)
            return true;
        else
            return false;
    }
    #endregion

    #region Set_Functions
    public void setPlcName(String plcName) { if (plcName != null) this.plcName = plcName; }

    public void setPlcIp(String plcIp) { this.plcIp = plcIp; }

    public void setPlcPort(int plcPort) { this.plcPort = plcPort; }

    public void setPlcRack(int plcRack) { this.plcRack = plcRack; }

    public void setPlcSlot(int plcSlot) { this.plcSlot = plcSlot; }
    #endregion

    #region Get_Functions
    public String getPlcName() { return this.plcName; }

    public String getPlcIp() { return this.plcIp; }

    public int getPlcPort() { return this.plcPort; }

    public int getPlcRack() { return this.plcRack; }

    public int getPlcSlot() { return this.plcSlot; }
    #endregion
}
