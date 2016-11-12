using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cecs545FinalProject
{
    class Log
    {
        const int MAX_RET_STR_LEN = 1000;

        private class Message
        {
            public string msg;
        }

        private List<Message> log;

        public Log()
        {

        }

        public void Write(string msgIn)
        {
            var newMsg = new Message() { msg = msgIn };
            log.Add(newMsg);
        }

        public string Read(int start, int end)
        {
            if (start + 30 < end)
            {
                throw new ArgumentException("May not request more than 30 entries at a time!");
            }

            string returnString = "";
            for(int i = start; i < end; i++)
            {
                returnString += log[start];
            }
            return returnString;
        }
    }
}
