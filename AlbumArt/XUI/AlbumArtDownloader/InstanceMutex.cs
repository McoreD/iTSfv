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
		[OperationContract(IsOneWay = true)]
		void Signal(string[] parameters);
		int Run();
	}

	public static class InstanceMutex
	{
		private static readonly TimeSpan WaitForFirstInstanceTimeout = TimeSpan.FromSeconds(3); //Maximum time between claiming to be the single instance, and starting to listen on the channel

		private static EventWaitHandle mNamedMutex; //Use this to avoid having to throw an exception on normal load behaviour
		private static bool mOwnsMutex; //True if this instance is the single instance that owns the mutex, and should listen on the channel

		/// <summary>
		/// Runs the application, and listens for signals from subsequent instances
		/// </summary>
		public static void RunAppAsServiceHost(IPriorInstance instance, string channelUri)
		{
			System.Diagnostics.Debug.Assert(mNamedMutex != null, "Expecting QueryPriorInstance to be called before RunAppAsServiceHost");

			if (mOwnsMutex)
			{
				ServiceHost service = new ServiceHost(instance, new Uri(channelUri));
				try
				{
					try
					{
						service.AddServiceEndpoint(typeof(IPriorInstance), new NetNamedPipeBinding(), new Uri(channelUri));
						service.Open();
						mNamedMutex.Set(); //Service is now listening
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
			else
			{
				// Does not own the mutex, so do not need to listen. Just run the app as a separate instance
				instance.Run();
			}
		}

		/// <summary>
		/// If a prior instance was running, sends the args to it and returns true. Otherwise, returns false.
		/// </summary>
		public static bool QueryPriorInstance(string[] args, string channelUri)
		{
			mNamedMutex = new EventWaitHandle(false, EventResetMode.ManualReset, channelUri, out mOwnsMutex);
			if (!mOwnsMutex) //No previous instance was running, if a new mutex was created.
			{
				//If a new mutex was not created, then wait for it to be signalled, indicating that the instance that does own it is now ready to recieve incoming signals
				if (mNamedMutex.WaitOne(WaitForFirstInstanceTimeout))
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
				else
				{
					System.Diagnostics.Trace.TraceWarning("Could not communicate with existing prior instance: timed out");
					return false;
				}
			}
			return false;
		}
	}
}