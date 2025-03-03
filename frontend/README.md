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
1. Run `npx expo export -p web` or `npm run deploy`

2. Upload dist folder to Netlify (currently only Nikita can do this because of Netlify free plan.)

3. View deployed website [here](https://hurovr.netlify.app/).
   

### Testing
1. Add a test in `__tests__` folder
2. Run all tests with `npm run test`

This project uses [file-based routing](https://docs.expo.dev/router/introduction).