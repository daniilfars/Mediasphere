import { Routes, Route } from 'react-router-dom';
import Home from '../pages/Home';
import Feed from '../pages/Feed';

export default function AppRouter() {
  return (
    <Routes>
      <Route path="/" element={<Feed />} />
      <Route path="/home" element={<Home />} />
    </Routes>
  );
}