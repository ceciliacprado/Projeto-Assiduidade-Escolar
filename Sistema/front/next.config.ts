import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  transpilePackages: ['@mui/material', '@emotion/react', '@emotion/styled'],
};

export default nextConfig;
