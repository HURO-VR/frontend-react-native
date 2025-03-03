// __tests__/firebase/auth.test.ts

import { signIn, signOut } from '../../firebase/auth';
import { initializeApp } from 'firebase/app';

// Mock Firebase
jest.mock('firebase/app');
jest.mock('firebase/auth');

describe('Authentication', () => {
  beforeEach(() => {
    // Clear mocks before each test
    jest.clearAllMocks();
  });

  it('signs in user successfully', async () => {
    const mockUser = { uid: '123', email: 'test@example.com' };
    (signInWithEmailAndPassword as jest.Mock).mockResolvedValue({ user: mockUser });

    const result = await signIn('test@example.com', 'password');
    expect(result).toEqual(mockUser);
  });
});
