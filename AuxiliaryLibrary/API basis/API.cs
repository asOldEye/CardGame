using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace AuxiliaryLibrary
{
    /// <summary>
    /// Основа для создания АПИ сервера
    /// </summary>
    public abstract class API
    {
        Dictionary<string, Action<APICommand>> apiCommands;

        public Action<object> sendObject;
        public Action<Stream, StreamInfo> sendStream;

        protected API()
        {
            apiCommands = new Dictionary<string, Action<APICommand>>()
            {
                { "GetAPICommands", new Action<APICommand>(GetAPICommands) },
                { "CompleteSession", new Action<APICommand>(CompleteSession) },
                { "GetAPIType", new Action<APICommand>(GetAPIType) }
            };
        }
        /// <summary>
        /// Инициализировать дополнительные команды АПИ, реализованные в классе-наследнике
        /// </summary>
        /// <param name="apiCommands">Дополнительные команды</param>
        protected void InitializeAPICommands(Dictionary<string, Action<APICommand>> apiCommands)
        {
            if (apiCommands != null)
                foreach (var f in apiCommands)
                    if (f.Value.Method.GetCustomAttributes(typeof(APICommandAttr), true).Length > 0)
                        this.apiCommands.Add(f.Key, f.Value);
                    else throw new ArgumentException("API command " + f.Key + " does not have an atribute [APICommandAttr]");
        }

        void Perform(APICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            
            if (apiCommands.TryGetValue(command.Command, out Action<APICommand> method))
            {
                Task t;
                var prms = command.Params;
                var attrPrms = (method.Method.GetCustomAttributes(typeof(APICommandAttr), true)[0] as APICommandAttr).InputParams;
                if (prms.Length != attrPrms.Length)
                {
                    SendObject(new APIAnswer(command, null,
                new NotImplementedException("Current API command, named " + command.Command + " must have " + attrPrms.Length + " parameters")));
                    return;
                }
                for (int i = 0; i < prms.Length; i++)
                    if (prms[i].GetType() != attrPrms[i])
                    {
                        string msg = "Current API command, named " + command.Command + " must have " + attrPrms.Length + " parameters of type: ";
                        foreach (var f in attrPrms) msg += "[" + f.GetType() + "]";
                        SendObject(new APIAnswer(command, null,
                new NotImplementedException("Current API command, named " + command.Command + " must have " + attrPrms.Length + " parameters")));
                        return;
                    }
                t = Task.Run(() => method(command));
            }
            else SendObject(new APIAnswer(command, null,
                new NotImplementedException("Current API haven't command, named " + command.Command)));
        }

        /// <summary>
        /// Операция, выполняемая при отключении
        /// </summary>
        public abstract void OnDisconnected();
        /// <summary>
        /// Рекция на принятый объект, по умолчанию только команды к АПИ
        /// </summary>
        /// <param name="obj">Принятый объект</param>
        public virtual void OnReceived(object obj)
        {
            if (obj is APICommand)
            {
                Perform(obj as APICommand);
                return;
            }
        }
        /// <summary>
        /// Реакция на входящий поток, по умолчанию реакция - дисконнект
        /// </summary>
        /// <param name="info">Входящий поток</param>
        public virtual void OnIncomingStream(StreamInfo info)
        {
            SendObject(new APIAnswer(new APICommand("SendStream", null), null,
                new NotImplementedException("Send stream are'nt supported by this API")));
            if (OnSessionEnded != null)
                OnSessionEnded.Invoke(this);
        }
        /// <summary>
        /// Продолжить текущую сессию другой
        /// </summary>
        /// <param name="other"></param>
        protected virtual void ContinueSession(API other)
        {
            if (other == null) throw new ArgumentException("Null other api");
            other.sendObject = sendObject;
            other.sendStream = sendStream;
            other.OnSessionContinued = OnSessionContinued;
            other.OnSessionEnded = OnSessionEnded;

            if (OnSessionContinued != null)
                OnSessionContinued.Invoke(this, other);
        }

        #region API commands
        [APICommandAttr]
        void GetAPICommands(APICommand command)
        {
            var dict = new Dictionary<string, Type[]>();
            foreach (var f in apiCommands)
                dict.Add(f.Key, (f.Value.Method.GetCustomAttributes(typeof(APICommandAttr), true)[0] as APICommandAttr).InputParams);
            SendObject(new APIAnswer(command, dict));
        }
        [APICommandAttr]
        void CompleteSession(APICommand command)
        {
            if (OnSessionEnded != null)
                OnSessionEnded.Invoke(this);
        }
        [APICommandAttr]
        void GetAPIType(APICommand command)
        {
            SendObject(new APIAnswer(command, GetType()));
        }
        #endregion

        protected void SendObject(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            sendObject.Invoke(obj);
        }
        protected void SendStream(Stream obj, StreamInfo info)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (info == null) throw new ArgumentNullException(nameof(info));
            sendStream.Invoke(obj, info);
        }

        public event ParametrizedEventHandler<API, API> OnSessionContinued;
        public event NonParametrizedEventHandler<API> OnSessionEnded;
    }
}