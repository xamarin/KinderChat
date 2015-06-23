using System;
using System.Collections.Generic;


namespace KinderChat
{
	/// <summary>
	/// A simple service container implementation, singleton only
	/// </summary>
	public static class ServiceContainer
	{
		static readonly Dictionary<Type, Lazy<object>> Services = new Dictionary<Type, Lazy<object>> ();
		static readonly Stack<Dictionary<Type, object>> ScopedServices = new Stack<Dictionary<Type, object>> ();

		/// <summary>
		/// Register the specified service with an instance
		/// </summary>
		public static void Register<T> (T service)
		{
			Services [typeof(T)] = new Lazy<object> (() => service);
		}

		/// <summary>
		/// Register the specified service for a class with a default constructor
		/// </summary>
		public static void Register<T> () where T : new()
		{
			Services [typeof(T)] = new Lazy<object> (() => new T ());
		}

		/// <summary>
		/// Register the specified service with a callback to be invoked when requested
		/// </summary>
		public static void Register<T> (Func<T> function)
		{
			Services [typeof(T)] = new Lazy<object> (() => function ());
		}

		/// <summary>
		/// Register the specified service with an instance
		/// </summary>
		public static void Register (Type type, object service)
		{
			Services [type] = new Lazy<object> (() => service);
		}

		/// <summary>
		/// Register the specified service with a callback to be invoked when requested
		/// </summary>
		public static void Register (Type type, Func<object> function)
		{
			Services [type] = new Lazy<object> (function);
		}

		/// <summary>
		/// Register the specified service with an instance that is scoped
		/// </summary>
		public static void RegisterScoped<T> (T service)
		{
			Dictionary<Type, object> services;
			if (ScopedServices.Count == 0) {
				services = new Dictionary<Type, object> ();
				ScopedServices.Push (services);
			} else {
				services = ScopedServices.Peek ();
			}

			services [typeof(T)] = service;
		}

		/// <summary>
		/// Resolves the type, throwing an exception if not found
		/// </summary>
		public static T Resolve<T> ()
		{
			return (T)Resolve (typeof(T));
		}

		/// <summary>
		/// Resolves the type, throwing an exception if not found
		/// </summary>
		public static object Resolve (Type type)
		{
			//Scoped services
			if (ScopedServices.Count > 0) {
				var services = ScopedServices.Peek ();

				object service;
				if (services.TryGetValue (type, out service)) {
					return service;
				}
			}

			//Non-scoped services
			{
				Lazy<object> service;
				if (Services.TryGetValue (type, out service)) {
					return service.Value;
				}
				throw new KeyNotFoundException (string.Format ("Service not found for type '{0}'", type));
			}
		}

		/// <summary>
		/// Adds a "scope" which is a way to register a service on a stack to be popped off at a later time
		/// </summary>
		public static void AddScope ()
		{
			ScopedServices.Push (new Dictionary<Type, object> ());
		}

		/// <summary>
		/// Removes the current "scope" which pops off some local services
		/// </summary>
		public static void RemoveScope ()
		{
			if (ScopedServices.Count > 0)
				ScopedServices.Pop ();
		}

		/// <summary>
		/// Mainly for testing, clears the entire container
		/// </summary>
		public static void Clear ()
		{
			Services.Clear ();
			ScopedServices.Clear ();
		}
	}
}