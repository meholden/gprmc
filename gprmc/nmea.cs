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
        int commacount, lastcomma, nextcomma;
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
            int ii,jj;
            String chksent, chkmade;
            int chkcalc;
            byte[] asciisent;
            ii = 0;
            for (ii = 0; ii < sentence.Length; ii++)
            {
                data = sentence.Substring(ii, 1);
                switch (data)
                {
                    case "$": // new sentence
                        nmea_buf = ""; // clear data
                        break;
                    case "\r": // end of sentence is  \r\n
                        // checksum calculation
                        if (nmea_buf.Substring(nmea_buf.Length - 3, 1).Equals("*"))
                        {
                            // checksum exists
                            chksent = nmea_buf.Substring(nmea_buf.Length - 2);
                            asciisent = Encoding.ASCII.GetBytes(nmea_buf);
                            // calculate checksum (Xor of chars between $ and *)
                            chkcalc = 0;
                            for (jj = 0; jj < nmea_buf.Length - 3; jj++)
                            {
                                chkcalc ^= asciisent[jj];
                            }
                            chkmade = chkcalc.ToString("X2");
                            if (chkmade.Equals(chksent))
                            {
                                nmeaparse();
                            }
                        }
                        break;
                    default: // everything else  
                        nmea_buf += data;
                        break;
                };  // switch data

            }
            return gotsentence;
        }

        private void nmeaparse()
        {
            int ii;
            commacount = 0;
            nextcomma = -1;
            for (ii = 0; ii < nmea_buf.Length; ii++)
            {
                data = nmea_buf.Substring(ii, 1);
                if (data == ",")
                {
                    // save comma locations in string
                    // there are probably string functions to do all this...
                    lastcomma = nextcomma;
                    nextcomma = ii;
                    // this substring is the data between the commas
                    nmea_buf2 = nmea_buf.Substring(lastcomma + 1, nextcomma - lastcomma - 1);
                    switch (commacount)
                    {
                        case 0: // 'GPRMC' 
                            rightsentence = nmea_buf2.Equals("GPRMC"); // is that what we have?
                            break;
                        case 2: // A=Good, V=bad
                            gpsgood = nmea_buf2.Equals("A"); //
                            break;
                        case 3: // lat   ddmm.mmm  
                            if (rightsentence && gpsgood)
                            {
                                lat = Convert.ToDouble(nmea_buf2.Substring(0, 2));
                                lat += Convert.ToDouble(nmea_buf2.Substring(2)) / 60d;
                            }
                            break;
                        case 4: // lat sign  
                            if (rightsentence)
                            {
                                if (nmea_buf2.Equals("S"))
                                {// default to northern hemisphere
                                    lat = -lat;
                                }
                            }
                            break;
                        case 5: // lon dddmm.mmm
                            if (rightsentence && gpsgood)
                            {
                                lon = Convert.ToDouble(nmea_buf2.Substring(0, 3));
                                lon += Convert.ToDouble(nmea_buf2.Substring(3)) / 60d;
                            }
                            break;
                        case 6: // lon sign 
                            if (rightsentence)
                            {
                                if (nmea_buf2.Equals("W"))
                                {// default to Eastern hemisphere
                                    lon = -lon;
                                }
                            }
                            break;
                        case 7: // sog XXX.X kt
                            if (rightsentence && gpsgood)
                            {
                                sog = Convert.ToDouble(nmea_buf2);
                            }
                            break;
                        case 8: // cog XXX.X deg     
                            if (rightsentence && gpsgood)
                            {
                                cog = Convert.ToDouble(nmea_buf2);
                                gotsentence = true;
                            }
                            break;
                    } // switch commacount 

                    commacount++;
                } // if ","
            }
        }

    }
}