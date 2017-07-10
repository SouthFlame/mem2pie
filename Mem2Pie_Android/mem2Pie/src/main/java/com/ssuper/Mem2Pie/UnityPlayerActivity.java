package com.ssuper.Mem2Pie;

import com.example.kimdongwoo_lab.unityplug.Gallery;
import com.unity3d.player.*;
import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.graphics.PixelFormat;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.provider.Settings;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Toast;

public class UnityPlayerActivity extends Activity {
	protected UnityPlayer mUnityPlayer; // don't change the name of this variable; referenced from native code
	public final static int REQUEST_CODE = -1010101;
	WindowManager mManager;
	View mView;
	LayoutInflater mInflater;
	final static int ACT_TEST = 0;
	boolean mFlag =false;
	final static int ACT_TEST2 = 1;

	static int STATE_VIEW= 0;
	// Setup activity layout
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		super.onCreate(savedInstanceState);

		getWindow().setFormat(PixelFormat.RGBX_8888); // <--- This makes xperia play happy

		mUnityPlayer = new UnityPlayer(this);
		setContentView(mUnityPlayer);
		mUnityPlayer.requestFocus();
		if (Build.VERSION.SDK_INT < Build.VERSION_CODES.LOLLIPOP) {
			//단말기 OS버전이 젤라빈 버전 보다 작을때.....처리 코드
		}
		else{
			checkDrawOverlayPermission(); //for G5
		}
		MakeView(getResources().getConfiguration().orientation == Configuration.
				ORIENTATION_PORTRAIT?1:0);
	}

	public void checkDrawOverlayPermission() {
		/** check if we already  have permission to draw over other apps */
		if (!Settings.canDrawOverlays(this)) {
			/** if not construct intent to request permission */
			Intent intent = new Intent(Settings.ACTION_MANAGE_OVERLAY_PERMISSION,
					Uri.parse("package:" + getPackageName()));
			/** request permission via start activity for result */
			startActivityForResult(intent, REQUEST_CODE);
		}
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		/** check if received result code
		 is equal our requested code for draw permission  */
		if (requestCode == REQUEST_CODE) {
			if (Settings.canDrawOverlays(this)) {
				// continue here - permission was granted
			}
		}
		else
		{
			switch(requestCode){
				case ACT_TEST:
					if (resultCode== RESULT_OK&&data!=null){
						destroy_VIEW();
						STATE_VIEW=3;
						String file_path = data.getStringExtra("file_path");
						Toast.makeText(this,  file_path , Toast.LENGTH_LONG).show();
						UnityPlayer.UnitySendMessage("Gallery", "OnPhotoPick", file_path);
					}
					break;
				case ACT_TEST2:
					//if (resultCode== RESULT_OK){
					destroy_VIEW();
					STATE_VIEW=3;

					//}
					break;
			}

		}
	}

	public void MakeView(int port) {

		mInflater = (LayoutInflater) getSystemService(this.LAYOUT_INFLATER_SERVICE);
		mView = mInflater.inflate(port==1?R.layout.layout_port:R.layout.layout_land, null);

		WindowManager.LayoutParams mParams = new WindowManager.LayoutParams(
				WindowManager.LayoutParams.MATCH_PARENT,
				WindowManager.LayoutParams.MATCH_PARENT,
				WindowManager.LayoutParams.TYPE_PHONE,
				WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE,

				PixelFormat.TRANSLUCENT);

		mManager = (WindowManager) getSystemService(WINDOW_SERVICE);
		mManager.addView(mView, mParams);
		STATE_VIEW=1;
	}

	// Quit Unity
	@Override
	protected void onDestroy() {
		mUnityPlayer.quit();
		super.onDestroy();
	}

	// Pause Unity
	@Override
	protected void onPause() {
		super.onPause();
		mUnityPlayer.pause();

	}

	//종료버튼이 한번 더 눌리지 않으면 mFlag 값 원복한다
	Handler mKillHandler = new Handler() {
		@Override
		public void handleMessage(Message msg) {
			if(msg.what == 0) {
				mFlag = false;
			}
		}
	};
	// Resume Unity
	@Override
	protected void onResume() {
		super.onResume();
		mUnityPlayer.resume();
		if(STATE_VIEW==0)
			MakeView(getResources().getConfiguration().orientation == Configuration.
					ORIENTATION_PORTRAIT?1:0);
		//
	}

	// This ensures the layout will be correct.
	@Override
	public void onConfigurationChanged(Configuration newConfig) {
		super.onConfigurationChanged(newConfig);
		mUnityPlayer.configurationChanged(newConfig);
		destroy_VIEW();
		switch(newConfig.orientation) {

			case Configuration.ORIENTATION_LANDSCAPE:
				MakeView(0);
				break;
			case Configuration.ORIENTATION_PORTRAIT:
				MakeView(1);

				break;
		}

	}

	// Notify Unity of the focus change.
	@Override
	public void onWindowFocusChanged(boolean hasFocus) {
		super.onWindowFocusChanged(hasFocus);
		mUnityPlayer.windowFocusChanged(hasFocus);


	}

	// For some reason the multiple keyevent type is not supported by the ndk.
	// Force event injection by overriding dispatchKeyEvent().
	@Override
	public boolean dispatchKeyEvent(KeyEvent event) {
		if (event.getAction() == KeyEvent.ACTION_MULTIPLE)
			return mUnityPlayer.injectEvent(event);
		return super.dispatchKeyEvent(event);
	}

	// Pass any events not handled by (unfocused) views straight to UnityPlayer
	@Override
	public boolean onKeyUp(int keyCode, KeyEvent event) {
		return mUnityPlayer.injectEvent(event);
	}

	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if(STATE_VIEW==1) {
			if (event.getAction() == KeyEvent.ACTION_DOWN) {

				if (keyCode == KeyEvent.KEYCODE_BACK) {
					//여기에 뒤로 버튼을 눌렀을때 해야할 행동을 지정한다
					if (!mFlag) {
						Toast.makeText(UnityPlayerActivity.this, "'뒤로'버튼을 한번 더 누르시면 종료됩니다.", Toast.LENGTH_SHORT).show();
						mFlag = true;
						mKillHandler.sendEmptyMessageDelayed(0, 2000);
						return false;
					} else {
						android.os.Process.killProcess(android.os.Process.myPid());
					}
				}
			}
		}
		else {
			return mUnityPlayer.injectEvent(event);
		}
		return false;
	}
	@Override
	public boolean onTouchEvent(MotionEvent event) {
		return mUnityPlayer.injectEvent(event);
	}

	/*API12*/
	public boolean onGenericMotionEvent(MotionEvent event) {
		return mUnityPlayer.injectEvent(event);
	}

	public void button(View v) {

		Intent intent = new Intent(getApplicationContext(), MainActivity.class);
		startActivityForResult(intent, ACT_TEST);
		destroy_VIEW();
	}

	public void button1(View v) {
		Intent intent = new Intent(getApplicationContext(), Gallery.class);
		startActivityForResult(intent,ACT_TEST2);
		destroy_VIEW();

	}
	public void button_setting(View v) {
		//TODO: 세팅 에러만 안나게 만들어놈

	}
	public void destroy_VIEW()
	{
		if(STATE_VIEW==1) {
			if (mManager != null) {        //서비스 종료시 뷰 제거. *중요 : 뷰를 꼭 제거 해야함.
				if (mView != null) mManager.removeView(mView);
			}
			STATE_VIEW = 0;
		}
	}

}
