package de.haw.run.simulationcore.modellcontainer;

import java.io.*;
import java.net.*;

import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;
import com.sun.net.httpserver.HttpServer;
import com.sun.net.httpserver.HttpContext;
import de.haw.run.GlobalTypes.Exceptions.BaseURLNotAvailableException;

public class RunHTTPServer {


    private static HttpServer server;
    private static HttpContext context;
    private static RunHTTPServer runHTTPServer;


    protected RunHTTPServer(int port, String rootPath) throws IOException {
        this.server = HttpServer.create(new InetSocketAddress(port), 0);
        this.context = server.createContext(rootPath, new MyHandler());
        server.setExecutor(null); // creates a default executor
        server.start();
    }


    public static void createWebServer(int port, String rootPath) {
        if(runHTTPServer == null){
            try {
                runHTTPServer = new RunHTTPServer(port, rootPath);
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }


    public static URI getBaseURL() throws BaseURLNotAvailableException {
        // TODO: Make sure right interface is selected on multi NIC hosts
        try {
            return new URI("http://" + InetAddress.getLocalHost().getHostAddress() + ":" + server.getAddress().getPort() + context.getPath());
        } catch (URISyntaxException | UnknownHostException e) {
            e.printStackTrace();
            throw new BaseURLNotAvailableException(e);
        }

    }


    static class MyHandler implements HttpHandler {
        public void handle(HttpExchange httpExchange) throws IOException {
            String filename = httpExchange.getRequestURI().getPath();
            String response = "";

            if(filename == null || filename.isEmpty()){
                filename = "error.html";
            }

            if(filename.startsWith("/")){
                filename = filename.substring(1);
            }

            File responseFile = new File(filename);
            if(!responseFile.isFile()){
                sendResponse(httpExchange, 404,"File Not found");
            } else {
                sendResponse(httpExchange, 200, responseFile);
            }
        }
    }

    static void sendResponse(HttpExchange httpExchange, int httpResponseCode, String body) throws IOException {
        byte[] data = body.getBytes();
        httpExchange.sendResponseHeaders(httpResponseCode, data.length);
        OutputStream os = httpExchange.getResponseBody();
        os.write(data, 0, data.length);
        os.close();
    }

    static void sendResponse(HttpExchange httpExchange, int httpResponseCode, File file) throws IOException {

        httpExchange.sendResponseHeaders(httpResponseCode, file.length());
        OutputStream os = httpExchange.getResponseBody();

        os.write(getBytesFromFile(file));
        os.close();
    }

    public static byte[] getBytesFromFile(File file) throws IOException {
        InputStream is = new FileInputStream(file);

        // Get the size of the file
        long length = file.length();

        if (length > Integer.MAX_VALUE) {
            // File is too large
        }

        // Create the byte array to hold the data
        byte[] bytes = new byte[(int)length];

        // Read in the bytes
        int offset = 0;
        int numRead = 0;
        while (offset < bytes.length
                && (numRead=is.read(bytes, offset, bytes.length-offset)) >= 0) {
            offset += numRead;
        }

        // Ensure all the bytes have been read in
        if (offset < bytes.length) {
            throw new IOException("Could not completely read file "+file.getName());
        }

        // Close the input stream and return bytes
        is.close();
        return bytes;
    }

}