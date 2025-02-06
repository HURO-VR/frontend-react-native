# HURO Frontend

This is an [Expo](https://expo.dev) project with deployment capabilities to mobile and web.

## Environment Setup:

1. **Install dependencies**

   ```bash
   npm install
   ```

2. **Start the app**

   ```bash
    npx expo start
   ```

3. **Open web view:**

   Press `w`


### Deploy Website
1. Run `npx expo export -p web`

2. Upload dist folder to Netlify (currently only Nikita can do this because of Netlify free plan.)

3. View deployed website [here](https://hurovr.netlify.app/).
   


- [development build](https://docs.expo.dev/develop/development-builds/introduction/)
- [Android emulator](https://docs.expo.dev/workflow/android-studio-emulator/)
- [iOS simulator](https://docs.expo.dev/workflow/ios-simulator/)
- [Expo Go](https://expo.dev/go), a sandbox for trying out app development with Expo

This project uses [file-based routing](https://docs.expo.dev/router/introduction).