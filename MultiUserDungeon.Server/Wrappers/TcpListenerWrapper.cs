using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiUserDungeon.Server
{
    /// <summary>
    /// Interface for important parts of TcpListener so that it can be faked.
    /// </summary>
    public interface ITcpListener
    {
        //
        // Summary:
        //     Gets or sets a System.Boolean value that specifies whether the System.Net.Sockets.TcpListener
        //     allows only one underlying socket to listen to a specific port.
        //
        // Returns:
        //     true if the System.Net.Sockets.TcpListener allows only one System.Net.Sockets.TcpListener
        //     to listen to a specific port; otherwise, false. . The default is true for Windows
        //     Server 2003 and Windows XP Service Pack 2 and later, and false for all other
        //     versions.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The System.Net.Sockets.TcpListener has been started. Call the System.Net.Sockets.TcpListener.Stop
        //     method and then set the System.Net.Sockets.Socket.ExclusiveAddressUse property.
        //
        //   T:System.Net.Sockets.SocketException:
        //     An error occurred when attempting to access the underlying socket.
        //
        //   T:System.ObjectDisposedException:
        //     The underlying System.Net.Sockets.Socket has been closed.
        bool ExclusiveAddressUse { get; set; }
        //
        // Summary:
        //     Gets the underlying System.Net.EndPoint of the current System.Net.Sockets.TcpListener.
        //
        // Returns:
        //     The System.Net.EndPoint to which the System.Net.Sockets.Socket is bound.
        EndPoint LocalEndpoint { get; }
        //
        // Summary:
        //     Gets the underlying network System.Net.Sockets.Socket.
        //
        // Returns:
        //     The underlying System.Net.Sockets.Socket.
        Socket Server { get; }
        //
        // Summary:
        //     Accepts a pending connection request.
        //
        // Returns:
        //     A System.Net.Sockets.Socket used to send and receive data.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The listener has not been started with a call to System.Net.Sockets.TcpListener.Start.
        Socket AcceptSocket();
        //
        // Summary:
        //     Accepts a pending connection request as an asynchronous operation.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task`1 The task object representing the asynchronous
        //     operation. The System.Threading.Tasks.Task`1.Result property on the task object
        //     returns a System.Net.Sockets.Socket used to send and receive data.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The listener has not been started with a call to System.Net.Sockets.TcpListener.Start.
        Task<Socket> AcceptSocketAsync();
        //
        // Summary:
        //     Enables or disables Network Address Translation (NAT) traversal on a System.Net.Sockets.TcpListener
        //     instance.
        //
        // Parameters:
        //   allowed:
        //     A Boolean value that specifies whether to enable or disable NAT traversal.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The System.Net.Sockets.TcpListener.AllowNatTraversal(System.Boolean) method was
        //     called after calling the System.Net.Sockets.TcpListener.Start method
        void AllowNatTraversal(bool allowed);
        //
        // Summary:
        //     Determines if there are pending connection requests.
        //
        // Returns:
        //     true if connections are pending; otherwise, false.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The listener has not been started with a call to System.Net.Sockets.TcpListener.Start.
        bool Pending();
        //
        // Summary:
        //     Starts listening for incoming connection requests.
        //
        // Exceptions:
        //   T:System.Net.Sockets.SocketException:
        //     Use the System.Net.Sockets.SocketException.ErrorCode property to obtain the specific
        //     error code. When you have obtained this code, you can refer to the Windows Sockets
        //     version 2 API error code documentation in MSDN for a detailed description of
        //     the error.
        void Start();
        //
        // Summary:
        //     Starts listening for incoming connection requests with a maximum number of pending
        //     connection.
        //
        // Parameters:
        //   backlog:
        //     The maximum length of the pending connections queue.
        //
        // Exceptions:
        //   T:System.Net.Sockets.SocketException:
        //     An error occurred while accessing the socket.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The backlog parameter is less than zero or exceeds the maximum number of permitted
        //     connections.
        //
        //   T:System.InvalidOperationException:
        //     The underlying System.Net.Sockets.Socket is null.
        void Start(int backlog);
        //
        // Summary:
        //     Closes the listener.
        //
        // Exceptions:
        //   T:System.Net.Sockets.SocketException:
        //     Use the System.Net.Sockets.SocketException.ErrorCode property to obtain the specific
        //     error code. When you have obtained this code, you can refer to the Windows Sockets
        //     version 2 API error code documentation in MSDN for a detailed description of
        //     the error.
        void Stop();
    }

    /// <summary>
    /// A straight wrapper for the TcpListener that implements the ITcpListener interface
    /// </summary>
    public class TcpListenerWrapper : TcpListener, ITcpListener
    {
        [System.Obsolete("Use TcpListener (IPAddress address, int port) instead")]
        public TcpListenerWrapper(int port) : base(port)
        {
        }

        public TcpListenerWrapper(IPEndPoint localEP) : base(localEP)
        {
        }

        public TcpListenerWrapper(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }
    }
}
