using System.Collections;

namespace Dinventory
{
    /// <summary>
    /// The abstract class that will be injected at the initialization of a Currency Purse 
    /// and will be used to store and load data.
    /// </summary>
    public abstract class CurrencyDataSaver
    {
        virtual public void Save(float ammount, CurrencyPurse.eCurrencyLevels level)
        {
            throw new System.NotImplementedException();
        }

        virtual public float LoadAmmount()
        {
            throw new System.NotImplementedException();
        }

        virtual public CurrencyPurse.eCurrencyLevels LoadLevel()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// This class its designed to manage an amount of a currency
    /// using a float and changing its value when a transactions(adding or substracting)
    /// happens.
    /// </summary>
    public class CurrencyPurse
    {

        /// <summary>
        /// The avaiable levels of the Purse: millions, billions, trillions, etc. 
        /// </summary>
        public enum eCurrencyLevels
        {
            none, K, m, b, t, q, Q
        }
        eCurrencyLevels level = eCurrencyLevels.none;
        public eCurrencyLevels Level
        {
            get
            {
                if (!isInitialized)
                    ThrowInitializedError();

                return level;
            }
        }

        /// <summary>
        /// The total amount stored at the purse
        /// </summary>
        float ammount = 0;
        public float Ammount
        {
            get
            {
                if (!isInitialized)
                    ThrowInitializedError();

                return ammount;
            }
        }

        /// <summary>
        /// Each time the purses adds or substracts an ammount it calls its injected saver,
        /// you can change this value to add how many times per transaction it calls its saver.
        /// </summary>
        public int SaveTicks = 1;
        int LastTick = 0;

        bool isInitialized = false;

        /// <summary>
        /// Reference to the injected saver. You must create a class that inherits CurrencyDateSaver
        /// and inject it when calling this class Init function before using it. 
        /// </summary>
        CurrencyDataSaver _saver;
        CurrencyDataSaver saver
        {
            get
            {
                if (!isInitialized)
                    ThrowInitializedError();

                return _saver;
            }
            set
            {
                _saver = value;
            }
        }

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="SaverInjection"> The saver to be used to I/O data</param>
        public CurrencyPurse(CurrencyDataSaver SaverInjection)
        {
            Init(SaverInjection);
        }


        /// <summary>
        /// This function loads the data from a class that inherits from CurrencyDataSaver.
        /// </summary>
        /// <param name="SaverInjection">The saver to be used to I/O data</param>
        void Init(CurrencyDataSaver SaverInjection)
        {
            if(!isInitialized)
            {
                isInitialized = true;
                saver = SaverInjection;
                ammount = saver.LoadAmmount();
                level = saver.LoadLevel();
            }
        }

        /// <summary>
        /// Forces the call of the saver ignoring the tick system.
        /// </summary>
        public void ForceSaveData()
        {
            LastTick = SaveTicks;
            SaveData();
        }

        /// <summary>
        /// In order to prevent calling a save each time the total amount changes you can say how many times per transaction the saver will be called.
        /// </summary>
        void SaveData()
        {
            if (LastTick >= SaveTicks)
            {
                saver.Save(ammount, level);
                LastTick = 0;
            }
            else
            {
                LastTick++;
            }
        }

        /// <summary>
        /// This function adds a value to the current ammount;
        /// </summary>
        /// <param name="ToAdd">quantity to add</param>
        ///  /// <param name="ToAddLevel">level of the quantitie to add</param>
        public void Add(float ToAdd, eCurrencyLevels ToAddLevel)
        {
            if(!isInitialized)
            {
                ThrowInitializedError();
                return;
            }

            int distance = ToAddLevel  - level;

            if(distance == 0)
            {
                ammount += ToAdd;
            }
            else if (distance == -1)
            {
                ammount += ToAdd / 1000;
            }else if(distance > 0)
            {
                ammount += ToAdd * (1000 * distance);
            }

            while(ammount > 1000)
            {
                level += 1;
                ammount /= 1000;
            }

            SaveData();

        }

        /// <summary>
        /// This function substracts a value from the current ammount;
        /// </summary>
        /// <param name="ToAdd">Quantity to sustract</param>
        ///  /// <param name="ToAddLevel">level of the quantitie to be substracted</param>
        public void Substarct(float ToRemove, eCurrencyLevels ToAddLevel)
        {
            if (!isInitialized)
            {
                ThrowInitializedError();
                return;
            }

            int distance = ToAddLevel - level;

            if (distance == 0)
            {
                ammount -= ToRemove;
            }
            else if (distance < 0)
            {
                ammount -= ToRemove / (1000 * distance);
            }
            else if (distance > 0)
            {
                ammount -= ToRemove * (1000 * distance);
            }

            while (ammount > 0  && ammount < 1)
            {
                level -= 1;
                ammount *= 1000;
            }

            SaveData();

        }

        void ThrowInitializedError()
        {
           // Removed this to forget about Unity Engine dependecy
           // Debug.LogError("The currency purse is not initialized");
        }


    }
}


