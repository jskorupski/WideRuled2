using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WideRuled2
{
    public class AblCommObject
    {

        private bool _doInteraction;
        private int _interactionId;
        private StringBuilder _currentOutput;
        private bool _storyFinished;
        private bool _abortStory;
        private bool _undo;
        private Queue<string> _storyTextQueue;


        private AblCommObject() { Constructor(); }

        private static AblCommObject _instance;

        public static AblCommObject Instance
        {

            get
            {

                lock (typeof(AblCommObject))

                {

                    if (_instance == null)

                        _instance = new AblCommObject();

                    lock (_instance)

                    {

                        return _instance;

                    }

                }

            }

        }

        void Constructor () 
        {
            _doInteraction = false;
            _interactionId = -1;
            _currentOutput = new StringBuilder();
            _storyFinished = false;
            _undo = false;
            _abortStory = false;
            _storyTextQueue = new Queue<string>();

        }


        public bool DoInteraction
        {
            get{return _doInteraction;}
            set{_doInteraction = value;}
        }

        public int InteractionId //Transient action, clear value when read
        {
            get 
            {
                _doInteraction = false;
                int oldId = _interactionId;
                _interactionId = -1;
                return oldId; 

            }
            set 
            {
                _doInteraction = true;
                _interactionId = value; 
            }
        }
    
        public void reset()
        {
            _doInteraction = false;
            _interactionId = -1;
            _currentOutput.Remove(0, _currentOutput.Length);
            _storyFinished = false;
            _undo = false;
            _abortStory = false;
            _storyTextQueue.Clear();
        }

        public String LatestText 
        {
            get { 
                if (_storyTextQueue.Count > 0)
                {
                    return _storyTextQueue.Dequeue();
                }
                else { return null; }
            }
            set {
                _storyTextQueue.Enqueue(value);
                StoryOutput.Append(value);
            }

        }

        public StringBuilder StoryOutput
        {
            get {return _currentOutput;}
            set { _currentOutput = value; }
        }

        public bool StoryFinished
        {
            get {return _storyFinished;}
            set { _storyFinished = value; }
        }

        public bool AbortStory
        {
            get { return _abortStory; }
            set { _abortStory = value; }
        }
        public bool Undo //Transient action, clear value to false when read 
        {
            get
            {
                if(_undo)
                {

                    _undo = false;
                    return true;
                }
                else
                {
                    return _undo;
                }


            }
            set
            {
                _undo = value;
            }
        }


    }
}
