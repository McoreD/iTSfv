//Code adapted from http://johnmelville.spaces.live.com/blog/cns!79D76793F7B6D5AD!122.entry
//Ensures only a single instance of this application runs, and passes command line arguments
//to the existing instance if a second instance is run.
using System;
using System.ServiceModel;
using System.Threading;

namespace AlbumArtDownloader
{

	[ServiceContract]
	public interface IPriorInstance
	{
		[OperationContract]
		void Signal(string[] parameters);
		int Run();
	}

	public static class InstanceMutex
	{
		private static Mutex mNamedMutex; //Use this to avoid having to throw an exception on normal load behaviour

		/// <summary>
		/// Runs the application, and listens for signals from subsequent instances
		/// </summary>
		public static void RunAppAsServiceHost(IPriorInstance instance, string channelUri)
		{
			System.Diagnostics.Debug.Assert(mNamedMutex != null, "Expecting QueryPriorInstance to be called before RunAppAsServiceHost");
			ServiceHost service = new ServiceHost(instance, new Uri(channelUri));
			try
			{
				try
				{
					service.AddServiceEndpoint(typeof(IPriorInstance), new NetNamedPipeBinding(), new Uri(channelUri));
					service.Open();
				}
				catch (CommunicationException ex)
				{
					System.Diagnostics.Trace.TraceWarning("Could not start listening as a prior instance: " + ex.Message);
					//Attempt a close, if at all possible
					try { service.Close(); }
					catch (Exception) { }
					service = null;
				}
				instance.Run();
			}
			finally
			{
				if (service != null)
					service.Close();
			}
			GC.KeepAlive(mNamedMutex); //Make sure the mutex sticks around until the app finishes running.
		}

		/// <summary>
		/// If a prior instance was running, sends the args to it and returns true. Otherwise, returns false.
		/// </summary>
		public static bool QueryPriorInstance(string[] args, string channelUri)
		{
			bool createdNew;
			mNamedMutex = new Mutex(true, channelUri, out createdNew);
			if (!createdNew) //No previous instance was running, if a new mutex was created.
			{
				try
				{
					EndpointAddress address = new EndpointAddress(channelUri);
					IPriorInstance instance = ChannelFactory<IPriorInstance>.CreateChannel(new NetNamedPipeBinding(), address);
					try
					{
						instance.Signal(args);
					}
					finally
					{
						((ICommunicationObject)instance).Close();
					}

					return true;
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.TraceWarning("Could not communicate with existing prior instance: " + ex.Message);
					return false;
				}
			}
			return false;
		}
	}
}