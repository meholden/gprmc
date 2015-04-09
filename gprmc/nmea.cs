using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gprmc
{
    public class nmea
    {
        String nmea_buf, nmea_buf2;
        String data;
        int commacount, digitcount ;
        Boolean rightsentence;
        public Boolean gpsgood, gotsentence;
        public Double lat, lon, sog, cog;
        // NMEA Decoding here
        // Wait for gps data, want RMC sentence:
        //$GPRMC,230046,A,3759.8006,N,12205.4429,W,0.0,190.3,260702,15.1,E,A*3C
        //              |     |    /     |      /   |   |
        //$GPRMC,233834,A,3759.842,N,12205.440,W,000.0,173.7,200602,015.8,E*63 
        //              |    |     |    |      |   |     |             |     |--checksum   
        //              |    |     |    |      |   |     |             |- Magnetic Deviation?
        //              |    |     |    |      |   |     |- course over ground         
        //              |    |     |    |      |   |- speed over ground
        //              |    |     |    |      |--Lon E/W
        //              |    |     |    |-- Lon 122' 5.44"
        //              |    |     |-- Lat N/S
        //              |    |-- Lat 37' 59.842"
        //              |-- A=Good, V=Bad  

        public Boolean processString(String sentence)
        {
            int ii;
            for (ii = 0; ii < sentence.Length; ii++) 
            {
                data = sentence.Substring(ii, 1);
                switch (data)
                {
                    case "$": // new sentence
                        commacount = 0;
                        digitcount = 0;
                        rightsentence = false;
                        nmea_buf = ""; // clear data
                        gpsgood = false;
                        break;
                    case ",": // comma
                        //nmea_buf[digitcount] = 0; // end string
                        // process nmea_buf
                        switch (commacount)
                        {
                            case 0: // 'GPRMC' 
                                rightsentence = nmea_buf.Equals("GPRMC"); // is that what we have?
                                break;
                            case 2: // A=Good, V=bad
                                gpsgood = nmea_buf.Equals("A"); //
                                break;
                            case 3: // lat   ddmm.mmm  
                                if (rightsentence)
                                {
                                    
                                    lat = Convert.ToDouble(nmea_buf.Substring(0,2));
                                    lat += Convert.ToDouble(nmea_buf.Substring(2)) / 60d;

                         
                                }
                                break;
                            case 4: // lat sign  
                                if (rightsentence)
                                {
                                    if (nmea_buf.Equals("S"))
                                    {// default to northern hemisphere
                                        lat = -lat;
                                    }
                                }
                                break;
                            case 5: // lon dddmm.mmm
                                if (rightsentence)
                                {
                                    lon = Convert.ToDouble(nmea_buf.Substring(0, 3));
                                    lon += Convert.ToDouble(nmea_buf.Substring(3)) / 60d;
                                 }
                                break;
                            case 6: // lon sign 
                                if (rightsentence)
                                {
                                    if (nmea_buf.Equals("W"))
                                    {// default to Eastern hemisphere
                                        lon = -lon;
                                    }
                                 }
                                break;
                            case 7: // sog XXX.X kt
                                if (rightsentence)
                                {
                                    sog = Convert.ToDouble(nmea_buf);
                                }
                                break;
                            case 8: // cog XXX.X deg     
                                if (rightsentence)
                                {
                                    sog = Convert.ToDouble(nmea_buf);
                                    gotsentence = true;
                                }
                                break;
                        } // switch commacount 
                        digitcount = 0;
                        nmea_buf = "";
                        commacount++;
                        break;
                    default: // everything else  
                        nmea_buf += data;
                        digitcount++;
                        if (digitcount > 19) digitcount = 0;
                        break;
                };  // switch data

            }
            return gotsentence;
        }                


    }
}
