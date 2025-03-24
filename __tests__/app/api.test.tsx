// Example API service test: __tests__/services/api.test.ts

import { fetchUserData } from '../../services/api';
  
jest.mock('../../services/api');

describe('API Service', () => {
  it('fetches user data successfully', async () => {
    const mockData = { id: 1, name: 'Test User' };
    (fetchUserData as jest.Mock).mockResolvedValue(mockData);
    
    const data = await fetchUserData(1);
    expect(data).toEqual(mockData);
  });
});