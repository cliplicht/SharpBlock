using SharpBlock.Server;

namespace SharpBlock.Pakets;

public class PaketFactory
{
    public IPaket? CreatePaket(int packetId, ConnectionState state)
    {
        switch (state)
        {
            case ConnectionState.Handshaking:
                if (packetId == 0x00) return new HandshakePaket();
                break;
            case ConnectionState.Status:
                if (packetId == 0x00) return new StatusRequestPaket();
                if (packetId == 0x01) return new PingRequestPaket();
                break;
            case ConnectionState.Login: 
                if (packetId == 0x00) return new LoginStartPaket();
                if (packetId == 0x01) return new EncryptionResponsePacket();
                // 0x02 == Login Plugin Response 
                // 0x03 Login Acknowledged
                // 0x04 Cookie Response (login)
                break;
            case ConnectionState.Configuration:
                if (packetId == 0x00) return null;
                break;
            case ConnectionState.Play:
                break;
            // Add other states and packets as needed
        }

        return null; // Or throw an exception
    }
}