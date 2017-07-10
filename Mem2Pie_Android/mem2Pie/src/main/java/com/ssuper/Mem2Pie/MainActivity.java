/*
 * Copyright 2016 LG Electronics Inc. All Rights Reserved.
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.ssuper.Mem2Pie;

import android.Manifest;
import android.app.Activity;
import android.app.ProgressDialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.media.ThumbnailUtils;
import android.net.Uri;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Environment;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v4.content.LocalBroadcastManager;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.Toast;

import com.lge.octopus.ConnectionManager;
import com.lge.octopus.OctopusManager;
import com.lge.octopus.tentacles.wifi.client.WifiClient;
import com.lge.osclibrary.HTTP_SERVER_INFO;
import com.lge.osclibrary.HttpAsyncTask;
import com.lge.osclibrary.OSCCommandsExecute;
import com.lge.osclibrary.OSCCommandsStatus;
import com.lge.osclibrary.OSCParameterNameMapper;

import org.json.JSONException;
import org.json.JSONObject;

import static com.ssuper.Mem2Pie.R.id.imageButton;

//Take Picture App
//Flow:
//  Start Session
//  Take Picture
//  Check status for take picture API
//  If take picture is done,
//       Close Session
//  else
//      keep check status for take picture API

public class MainActivity extends Activity {
    private static final int REQUEST_EXTERNAL_STORAGE = 1;
    private static String[] PERMISSIONS_STORAGE = {
            Manifest.permission.READ_EXTERNAL_STORAGE,
            Manifest.permission.WRITE_EXTERNAL_STORAGE
    };
    private EditText editTextIPAddress;
    private Button buttonTakePicture;
    private ImageButton Unitygo;

    private Button buttonConnect;
    private boolean keepSendRequest;
    private ImageView imagePreView;
    private Context mContext;
    private static final String IMAGE = "image";
    private static final String VIDEO = "video";
    String dcimDirectory;
    private boolean startDownloading; //Check whether it is downloading or not
    private ProgressDialog mProgressDialog;
    private String mediaType=IMAGE;
    private String currentDownloadFile; //Recent download file name
    private String PATH;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        editTextIPAddress = (EditText) findViewById(R.id.editTextIPAddr);
        buttonTakePicture = (Button) findViewById(R.id.button_takePicture);
        buttonConnect = (Button) findViewById(R.id.button_connect);
        imagePreView = (ImageView) findViewById(R.id.imageView_preview);

        Unitygo= (ImageButton) findViewById(imageButton);
        //Set IP for http request
        String IP = editTextIPAddress.getText().toString();
        setIPPort(IP);
        mContext = this;

        // register local broadcast receiver
        LocalBroadcastManager.getInstance(getApplicationContext())
                .registerReceiver(mReceiver, getFilter());

        //Send 'take picture' request to camera
        //Need to start session before taking picture
        buttonTakePicture.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                takePicture();
            }
        });
        Unitygo.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                UnityGO(v);
            }
        });
        dcimDirectory = Environment.getExternalStoragePublicDirectory
                (Environment.DIRECTORY_DCIM).toString();
        dcimDirectory += "/friendsCamera";
        getLivePreview();

        checkFileWritePermission();
        buttonConnect.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {
                String currentConnection = buttonConnect.getText().toString();
                if (currentConnection.equals(getString(R.string.connect))) {
                    Intent i = new Intent(mContext, ConnectionActivity.class);
                    startActivity(i);
                } else {
                    disconnectWifi();
                }
            }
        });
    }

    public boolean checkIsConnectedToDevice() {
        WifiManager wifimanager;
        wifimanager = (WifiManager) getSystemService(Context.WIFI_SERVICE);

        WifiInfo info = wifimanager.getConnectionInfo();

        String ssid = info.getSSID();

        if(ssid.contains(".OSC")){
            return true;
        }
        else{
            return false;
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        //ConnectionManager mConnectionManager = OctopusManager.getInstance(mContext).getConnectionManager();
        if(checkIsConnectedToDevice()){
            buttonTakePicture.setEnabled(true);
            buttonConnect.setText(R.string.disconnect);
        }else {
            buttonTakePicture.setEnabled(false);
            buttonConnect.setText(R.string.connect);
        }

        keepSendRequest = true;
    }

    @Override
    protected void onDestroy() {
        LocalBroadcastManager.getInstance(getApplicationContext())
                .unregisterReceiver(mReceiver);
        super.onDestroy();
    }

    // disconnect wifi
    private void disconnectWifi(){
        ConnectionManager mConnectionManager = OctopusManager.getInstance(mContext).getConnectionManager();
        mConnectionManager.disconnect();
    }

    // make intent filter
    private IntentFilter getFilter(){
        IntentFilter mFilter = new IntentFilter();
        mFilter.addAction(WifiClient.ACTION_WIFI_STATE);
        return mFilter;
    }

    private void setIPPort(String ip) {
        String[] temp = ip.split(":");
        HTTP_SERVER_INFO.IP = temp[0];
        if (temp.length == 2) {
            HTTP_SERVER_INFO.PORT = temp[1];
        } else {
            HTTP_SERVER_INFO.PORT = "6624";
        }
    }

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            final String action = intent.getAction();
            int result = intent.getIntExtra(WifiClient.EXTRA_RESULT, WifiClient.RESULT.DISCONNECTED);
            if(WifiClient.ACTION_WIFI_STATE.equals(action)){
                if(result == WifiClient.RESULT.CONNECTED){
                    buttonConnect.setText(R.string.disconnect);
                    buttonTakePicture.setEnabled(true);
                } else {
                    buttonConnect.setText(R.string.connect);
                    buttonTakePicture.setEnabled(false);
                }
            }
        }
    };

    /*
        Send 'take picture' request to camera
        - OSC Protocol: osc/commands/execute
        - OSC API: camera.takePicture
        - Parameter for 'camera.takePicture':
            {
                "parameters": {
                "sessionId": session ID
                }
            }

        - Use OSCCommandExecute class for /osc/commands/execute
        - OSCCommandsExecute(String command_name, JSONObject parameters);
    */
    private void takePicture() {
        //Set parameter for camera.takePicture API
        //Make parameter by using JSONObject
        OSCCommandsExecute commandsExecute = new OSCCommandsExecute("camera.takePicture", null);
        commandsExecute.setListener(new HttpAsyncTask.OnHttpListener() {
            @Override
            public void onResponse(OSCReturnType type, Object response) {

                try {
                    JSONObject jObject = new JSONObject((String) response);
                    String state;

                    //Get normal response
                    if (jObject.has(OSCParameterNameMapper.COMMAND_STATE)) {
                        state = jObject.getString(OSCParameterNameMapper.COMMAND_STATE);
                        //Taking picture is on progress
                        if (state.equals(OSCParameterNameMapper.STATE_INPROGRESS)) {
                            String commandId = jObject.getString(OSCParameterNameMapper.COMMAND_ID);
                            checkCommandsStatus(commandId);
                        }
                        //Taking picture is finished
                        else {
                            handleFinishTakePicture((String) response);
                        }
                    }
                    //Get error response
                    else {
                        Toast.makeText(mContext, "[Error] Error occur during taking picture", Toast.LENGTH_LONG).show();
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }

            }
        });
        commandsExecute.execute();
    }


    /*
        Check previous api status
        - OSC Protocol: osc/commands/status
        - Parameter for /osc/commands/status: command ID(get from previous /osc/commands/execute)
        - Use OSCCommandStatus class for /osc/commands/status
        - OSCCommandsStatus(String command_id);
    */
    private void checkCommandsStatus(final String commandId) {
        OSCCommandsStatus commandsStatus = new OSCCommandsStatus(commandId);
        commandsStatus.setListener(new HttpAsyncTask.OnHttpListener() {
            @Override
            public void onResponse(OSCReturnType type, final Object response) {

                try {
                    JSONObject jObject = new JSONObject((String) response);
                    String state;
                    //Get normal response
                    if (jObject.has(OSCParameterNameMapper.COMMAND_STATE)) {
                        state = jObject.getString(OSCParameterNameMapper.COMMAND_STATE);
                        //Taking picture is on progress
                        if (state.equals(OSCParameterNameMapper.STATE_INPROGRESS)) {
                            checkCommandsStatus(commandId);
                        }
                        //Taking picture is finished
                        else {
                            handleFinishTakePicture((String) response);
                        }
                    }
                    //Get error response
                    else {
                        Toast.makeText(mContext, "Error occur during taking picture", Toast.LENGTH_LONG).show();
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }

            }
        });
        commandsStatus.execute();
    }


    private void handleFinishTakePicture(String response) {
        //Finish take picture, then show toast and close session
        String fileUrl = null;
        try {
            JSONObject jObject = new JSONObject(response);

            if (jObject.has(OSCParameterNameMapper.RESULTS)) {
                JSONObject results = jObject.getJSONObject(OSCParameterNameMapper.RESULTS);

                fileUrl = results.getString(OSCParameterNameMapper.FileInfo.URL);

                getFullFile(fileUrl); //image download;
            }
        } catch (JSONException e) {
            e.printStackTrace();
        }

        if (fileUrl != null) {
            //Toast.makeText(mContext, "Picture " + fileUrl + " is taken", Toast.LENGTH_LONG).show();
        } else {
            Toast.makeText(mContext, "[Error] Take picture response Error", Toast.LENGTH_LONG).show();
        }
    }
    private void getLivePreview() {
        OSCCommandsExecute commandsExecute = new OSCCommandsExecute("camera.getLivePreview", null,
                OSCCommandsExecute.CommandType.PREVIEW);
        commandsExecute.setListener(new HttpAsyncTask.OnHttpListener() {
            @Override
            public void onResponse(OSCReturnType type, Object response) {
                if (type == OSCReturnType.SUCCESS) {
                    imagePreView.setImageBitmap((Bitmap) response);
                    if(keepSendRequest){
                        getLivePreview();
                    }
                }
                else{
                    keepSendRequest = false;
                }
            }
        });
        commandsExecute.execute();
    }
    private void getFullFile(final String fileUrl) {

        currentDownloadFile = fileUrl;

        JSONObject parameters = new JSONObject();
        try {
            String url = fileUrl;
            parameters.put(OSCParameterNameMapper.FILEURL, url);

            OSCCommandsExecute commandsExecute;

            //Set the data type for request (image or video)
            //It will set different http request header property
            if (mediaType.equals(IMAGE)) {
                commandsExecute = new OSCCommandsExecute("camera.getFile", parameters,
                        OSCCommandsExecute.CommandType.IMAGE);
            } else if (mediaType.equals(VIDEO)) {
                commandsExecute = new OSCCommandsExecute("camera.getFile", parameters,
                        OSCCommandsExecute.CommandType.VIDEO);
            } else {
                return;
            }
            commandsExecute.setListener(new HttpAsyncTask.OnHttpListener() {
                @Override
                public void onResponse(HttpAsyncTask.OnHttpListener.OSCReturnType type, Object response) {
                    if (type == OSCReturnType.SUCCESS) {
                        //Get binary data from camera and save successfully
                        //Response of getFile is the fileUri of the saved file
                        String name =     fileUrl.substring( fileUrl.lastIndexOf('/')+1, fileUrl.length() );
                        handleResponse(name, (String) response);
                        PATH=name;
                        ImageButton iv = (ImageButton) findViewById(imageButton);
                        final int THUMBSIZE = 240;
                        iv.setImageBitmap(ThumbnailUtils.extractThumbnail(BitmapFactory.decodeFile(dcimDirectory+"/"+PATH),
                                THUMBSIZE, THUMBSIZE));
                    } else {
                        //    Utils.showTextDialog(mContext, getString(R.string.response), Utils.parseString(response));
                    }
                }
            });
            commandsExecute.execute();
            //Set downloading flag as true
            startDownloading = true;
            mProgressDialog = ProgressDialog.show(mContext, "", "Downloading...", true, false);
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    private void handleResponse(String fileName, String localUri) {
        //Set downloading flag as false
        //Update android gallery
        startDownloading = false;
        currentDownloadFile = "";
        mContext.sendBroadcast(new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE, Uri.parse("file://" + (String) localUri)));
        Toast.makeText(mContext, fileName + " is saved", Toast.LENGTH_LONG).show();
        mProgressDialog.cancel();
    }
    private final int MY_PERMISSION_REQUEST_WRITE_EXTERNAL_STORAGE = 1;
    private void checkFileWritePermission() {
        int permissionCheck = ContextCompat.checkSelfPermission(mContext, Manifest.permission.WRITE_EXTERNAL_STORAGE);
        if (permissionCheck != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions((Activity) mContext, new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE},
                    MY_PERMISSION_REQUEST_WRITE_EXTERNAL_STORAGE);
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        switch (requestCode) {
            case MY_PERMISSION_REQUEST_WRITE_EXTERNAL_STORAGE:
                if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    //     Log.d(TAG, "Permission for write external storage is granted");
                } else {
                    //     Log.d(TAG, "Permission for write external storage is denied");
                }
        }
    }

    public void UnityGO(View v) {

        if (PATH != null){
            //Toast.makeText(mContext,   dcimDirectory +"/"+ PATH , Toast.LENGTH_LONG).show();
            Intent intent = getIntent();
            intent.putExtra("file_path",  dcimDirectory +"/"+ PATH);
            setResult(RESULT_OK, intent);
            PATH=null;
            finish();
        }
    }
}