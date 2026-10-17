import { Routes, Route } from 'react-router-dom';
import Profile from '../pages/Profile';
import Feed from '../pages/Feed';

export default function AppRouter() {
  return (
    <Routes>
      <Route path="/" element={<Feed />} />
      <Route path="/profile/:id" element={<Profile />} />
    </Routes>
  );
}