using System;
using System.Collections.Generic;
using AuxiliaryLibrary;
using CardSessionShared;

namespace CardSessionServer
{
    /// <summary>
    /// Контейнер для объектов
    /// </summary>
    [Interptered("SessionObject")]
    [Serializable]
    public abstract class Container
    {
        /// <summary>
        /// Идентификатор в сессии
        /// </summary>
        [Interptered("ID")]
        public int ID { get; private set; }
        /// <summary>
        /// Владелец этого объекта
        /// </summary>
        [Interptered("Owner")]
        public IControllerInfo Owner { get; private set; }
        /// <summary>
        /// Установить владельца
        /// </summary>
        public virtual void SetOwner(IControllerInfo owner)
        { Owner = owner; }
        /// <summary>
        /// Список модификаторов объекта
        /// </summary>
        [Interptered("Modifiers")]
        public List<DurableModifier> Modifiers { get; } = new List<DurableModifier>();
        /// <summary>
        /// Сессия, в которой находится объект
        /// </summary>
        [Interptered("Session")]
        public Session Session { get; private set; }
        /// <summary>
        /// Добавить объект в сессию/убрать (s=null)
        /// </summary>
        public virtual void SetSession(Session s, int id = -1)
        {
            if (Session != null && (Session = s) == null)
            {
                ID = -1;
                if (OnVanished != null) OnVanished.Invoke(this);
            }
            else if (Session == null && (Session = s) != null)
            {
                ID = id;
                if (OnAppears != null) OnAppears.Invoke(this);
            }
        }

        List<Component> components = new List<Component>();
        /// <summary>
        /// Возвращает компоненты
        /// </summary>
        public T GetComponent<T>() where T : Component
        {
            var comp = components.Find(f => f.GetType() == typeof(T));
            if (comp != null) return comp as T;
            return null;
        }
        public Component GetComponent(Type T)
        { return components.Find(f => f.GetType() == T); }
        /// <summary>
        /// Добавляет компонент
        /// </summary>
        public void AddComponent<T>(T component) where T : Component
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            if (GetComponent<T>() != null) throw new ArgumentException("Already contain component " + component.GetType());
            components.Add(component);
            component.Appear(this);
        }
        /// <summary>
        /// Удаляет компонент
        /// </summary>
        public bool DelComponent<T>(T component) where T : Component
        {
            if (GetComponent<T>() == null) return false;
            components.Remove(component);
            component.Wanish();
            return true;
        }
        public List<Component> GetComponents<T>() where T: Component
        { return components.FindAll(f => f.GetType() == typeof(T)); }
        public List<Component> GetComponentsOf<T>() where T : Component
        { return components.FindAll(f => f is T); }

        public NonParametrizedEventHandler<Container> OnAppears;
        public NonParametrizedEventHandler<Container> OnVanished;
    }
}