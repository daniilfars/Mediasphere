import { Routes, Route } from 'react-router-dom';
import Profile from '../pages/Profile';
import Feed from '../pages/Feed';
import EditPost from '../pages/EditPost';

export default function AppRouter() {
  return (
    <Routes>
      <Route path="/" element={<Feed />} />
      <Route path="/profile/:id" element={<Profile />} />
      <Route path="/post/:id/edit" element={<EditPost />} />
    </Routes>
  );
}